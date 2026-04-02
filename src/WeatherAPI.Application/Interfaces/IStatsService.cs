using WeatherAPI.Application.DTOs;

namespace WeatherAPI.Application.Interfaces;

public interface IStatsService
{
    public Task<IReadOnlyList<TopCityDto>> GetTopCitiesAsync(DateOnly from, DateOnly to, int limit, CancellationToken cancellationToken);
    public Task<PagedResult<RequestLogDto>> GetRequestsAsync(DateOnly from, DateOnly to, int page, int pageSize, CancellationToken cancellationToken);
}