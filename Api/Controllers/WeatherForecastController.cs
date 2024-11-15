using Api.MessageBroker;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("weatherforecast")]
public class WeatherForecastController : ControllerBase
{
    private readonly Exchange _exchange;
    public WeatherForecastController(Exchange exchange)
    {
        _exchange = exchange;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
    {
        Console.WriteLine("WeatherForecast Get endpoint called");
        
        // Send request through message broker
        await _exchange.PublishWeatherRequest();
        
        // TODO: Implement response handling
        // For now, returning dummy data
        var forecast = GenerateDummyForecast();
        return Ok(forecast);
    }

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Skibidi", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

    private static WeatherForecast[] GenerateDummyForecast()
    {
        return Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                Summaries[Random.Shared.Next(Summaries.Length)]
            )).ToArray();
    }
}