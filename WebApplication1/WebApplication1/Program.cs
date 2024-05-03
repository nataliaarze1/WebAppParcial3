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
// Agregar la configuración de FilePatientStorage
builder.Services.AddSingleton<FilePatientStorage>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var filePath = configuration.GetSection("FileStorage")["PatientDataFilePath"];
    return new FilePatientStorage(filePath);
});

var logConfiguration = new LoggerConfiguration();
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    logConfiguration.WriteTo.Console();
    logConfiguration.WriteTo.File(Path.Combine(builder.Configuration["Logging:FileLocation"], "log.txt"), rollingInterval: RollingInterval.Day);
    Log.Logger = logConfiguration.CreateLogger();
    Log.Information($"Initializing the server in the enviromment {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}!!");
}
else {
    logConfiguration.WriteTo.File(Path.Combine(builder.Configuration["Logging:FileLocation"], "log.txt"), rollingInterval: RollingInterval.Day);
    Log.Logger = logConfiguration.CreateLogger();
    Log.Information($"Initializing the server in the enviromment {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}!!");
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<IPatientsManager, PatientManager>();
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

