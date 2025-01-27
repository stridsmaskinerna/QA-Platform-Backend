using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Presentation.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<TestController> _logger;
    private readonly IBaseService _baseService;

    public TestController(
        ILogger<TestController> logger,
        IBaseService baseService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
        _baseService = baseService ?? throw new ArgumentNullException(nameof(baseService)); ;
    }

    [HttpGet("error")]
    public IEnumerable<WeatherForecast> GetError()
    {
        _baseService.Unauthorized();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}

