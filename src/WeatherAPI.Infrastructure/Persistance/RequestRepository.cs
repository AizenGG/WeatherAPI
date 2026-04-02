using Microsoft.EntityFrameworkCore;
using WeatherAPI.Application.DTOs;
using WeatherAPI.Application.Interfaces;
using WeatherAPI.Domain.Entites;

namespace WeatherAPI.Infrastructure.Persistance;

public class RequestRepository : IRequestRepository
{
    private readonly AppDbContext _dbContext;

    public RequestRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task SaveAsync(RequestLog log, CancellationToken cancellationToken)
    {
        _dbContext.Requests.Add(log);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TopCityDto>> GetTopCitiesAsync(DateOnly from, DateOnly to, int limit, CancellationToken cancellationToken)
    {
        var fromUtc = DateTime.SpecifyKind(from.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(to.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc);

        return await _dbContext.Requests
            .Where(r => r.TimestampUtc >= fromUtc && r.TimestampUtc <= toUtc)
            .GroupBy(r => r.City)
            .Select(g => new TopCityDto
            {
                City = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(r => r.Count)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<RequestLogDto>> GetRequestAsync(DateOnly from, DateOnly to, int page, int pageSize, CancellationToken cancellationToken)
    {
        var fromUtc = DateTime.SpecifyKind(from.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(to.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc);

        var query = _dbContext.Requests
            .Where(r => r.TimestampUtc >= fromUtc && r.TimestampUtc <= toUtc);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.TimestampUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new RequestLogDto
            {
                Id = r.Id,
                TimestampUtc = r.TimestampUtc,
                Endpoint = r.Endpoint.ToString(),
                Date = r.Date,
                City = r.City,
                CacheHit = r.CacheHit,
                StatusCode = r.StatusCode,
                LatencyMs = r.LatencyMs
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<RequestLogDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }
}