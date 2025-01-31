using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddSerilog();

builder.Services.AddOpenApi();

builder.Host.UseSerilog((_, services, config) =>
{
    config.ReadFrom.Configuration(builder.Configuration);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (ILogger<WeatherForecast> logger) =>
{



    try
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
       .ToArray();
        forecast.ToList().ForEach(a =>

        logger.LogInformation("The temprature is {Temprature} and the weather will be {WeatherType}", a.TemperatureC, a.Summary));
        return forecast;
    }
    catch (Exception)
    {
        throw;
    }
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
