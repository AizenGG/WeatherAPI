using WeatherAPI.Application.DTOs;
using WeatherAPI.Domain.Entites;
using WeatherAPI.Domain.Enums;

namespace WeatherAPI.Application.Interfaces;

public interface IRequestRepository
{
    public Task SaveAsync(RequestLog log, CancellationToken cancellationToken);
    public Task<IReadOnlyList<TopCityDto>> GetTopCitiesAsync(DateOnly from, DateOnly to, int limit, CancellationToken cancellationToken);
    public Task<PagedResult<RequestLogDto>> GetRequestAsync(DateOnly from, DateOnly to, int page, int pageSize, CancellationToken cancellationToken);
}