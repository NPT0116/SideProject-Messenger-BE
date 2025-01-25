using System.Text;
using System.Text.Json.Serialization;
using Application;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Seed;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using StackExchange.Redis;
using WebApi.Hubs;
using WebApi.Middlewares;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sushi Restaurant API",
        Version = "v1",
        Description = "API for Sushi Restaurant Management System"
    });
    c.OperationFilter<FileUploadOperationFilter>();

    // Adding Authentication for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddSignalR();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();

// Configure Redis
var RedisConnection = builder.Configuration.GetConnectionString("RedisConnection");
Log.Information("Using RedisConnection string: {RedisConnection}", RedisConnection);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "WebApi_";
});

// Register ConnectionMultiplexer
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection"))
);

// global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo.Console();
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddAuthorization();

// Add this section to conditionally use appsettings.json for Local environment
if (builder.Environment.IsEnvironment("Local"))
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
}

// Register the background service
builder.Services.AddHostedService<LastSeenSyncService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // Replace with your client URL
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Log.Information("Using connection string: {ConnectionString}", connectionString);

var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference(options => {
        options.WithTitle("Messenger API")
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.Axios);
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization(); 
app.UseExceptionHandler();
app.UseMiddleware<UpdateLastAccessMiddleware>(); // Enable the middleware
app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

// Map SignalR hubs
app.MapHub<ChatHub>("/chathub");

app.Run();