using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Application.Interfaces;

namespace WeatherAPI.Controllers;
[ApiController]
[Route("/api/weather/")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }
    
    [HttpGet("{city}")]
    public async Task<IActionResult> GetByDate(string city, [FromQuery] DateOnly date,CancellationToken token)
    {
        var response = await _weatherService.GetWeatherDayAsync(city, date, token);
        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }
    [HttpGet("{city}/week")]
    public async Task<IActionResult>  GetForWeek(string city,CancellationToken token)
    {
        var response = await _weatherService.GetWeatherWeekAsync(city, token);
        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }
}