using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Application.Interfaces;

namespace WeatherAPI.Controllers;
[ApiController]
[Route("/api/stats/")]

public class StatsController : ControllerBase
{
    private readonly IStatsService _statsService;

    public StatsController(IStatsService statsService)
    {
        _statsService = statsService;
    }
    
    [HttpGet("top-cities")]
    public async Task<IActionResult> GetTopCities([FromQuery] DateOnly from, [FromQuery] DateOnly to, [FromQuery] int limit = 10, CancellationToken token = default)
    {
        var response = await _statsService.GetTopCitiesAsync(from, to, limit, token);
        return Ok(response);
    }

    [HttpGet("requests")]
    public async Task<IActionResult> GetRequests([FromQuery] DateOnly from, [FromQuery] DateOnly to, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken token = default)
    {
        var response = await _statsService.GetRequestsAsync(from, to, page, pageSize, token);
        return Ok(response);
    }
}