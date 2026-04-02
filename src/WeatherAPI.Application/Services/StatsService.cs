using WeatherAPI.Application.DTOs;
using WeatherAPI.Application.Interfaces;

namespace WeatherAPI.Application.Services;

public class StatsService : IStatsService
{
    private readonly IRequestRepository _requestRepository;

    public StatsService(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public Task<IReadOnlyList<TopCityDto>> GetTopCitiesAsync(DateOnly from, DateOnly to, int limit,
        CancellationToken cancellationToken)
        => _requestRepository.GetTopCitiesAsync(from, to, limit, cancellationToken);

    public Task<PagedResult<RequestLogDto>> GetRequestsAsync(DateOnly from, DateOnly to, int page, int pageSize, CancellationToken cancellationToken)
        => _requestRepository.GetRequestAsync(from, to, page, pageSize, cancellationToken);
    
}