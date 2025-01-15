
using Application;
using Infrastructure;
using Scalar.AspNetCore;
using Serilog;
using WebApi.Middlewares;

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

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo.Console();
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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





