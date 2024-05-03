using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using businesslogic;
using Serilog;
using System.Diagnostics;
using WebApplication1.Middleware;



var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(builder.Configuration["Logging:FileLocation"], "log.txt"), rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Information("Initializing the server!!");


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<IPatientsManager, StudentManager>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandlerMiddleware(); // Aquí se usa el middleware personalizado
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// RUN
app.Run();

