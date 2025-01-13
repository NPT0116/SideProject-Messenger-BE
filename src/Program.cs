using backend.src.Data;
using backend.src.Exceptions.Example;
using backend.src.Middlewares;
using backend.src.Validators.User;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// serilog


// global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });
builder.Services.AddValidatorsFromAssemblyContaining<UserRegistorRequestValidator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
        app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference(options => {
       options.WithTitle ("Messenger API")
       .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.Axios);

    });
}
app.MapGet("/", () => { throw new ProductNotFoundException(Guid.NewGuid()); });app.UseHttpsRedirection();
app.UseExceptionHandler();


app.Run();





