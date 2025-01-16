// filepath: /C:/Users/Admin/Desktop/web_messenger/WebApi/Program.cs
using Application;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Seed;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
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

// Add this section to conditionally use appsettings.json for Local environment
if (builder.Environment.IsEnvironment("Local"))
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Log.Information("Using connection string: {ConnectionString}", connectionString);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //await context.Database.MigrateAsync();
    // await DatabaseSeed.SeedData(context);
}

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

//app.MapGet("/", () => { throw new ProductNotFoundException(Guid.NewGuid()); });
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapControllers();

app.Run();