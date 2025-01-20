// filepath: /C:/Users/Admin/Desktop/web_messenger/WebApi/Program.cs
using System.Text;
using Application;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Realtime;
using Infrastructure.Seed;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
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

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();

// global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
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


// Add cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Log.Information("Using connection string: {ConnectionString}", connectionString);

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     //await context.Database.MigrateAsync();
//     await DatabaseSeed.SeedData(context);
// }

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
app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization(); 
app.UseExceptionHandler();
app.UseMiddleware<UpdateLastAccessMiddleware>();
app.MapControllers();

app.MapHub<VideoCallHub>("/videocallhub");

app.Run();