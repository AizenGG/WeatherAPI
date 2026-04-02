using WeatherAPI.Domain.Models;

namespace WeatherAPI.Application.Interfaces;

public interface IGeocodingService
{
    public Task<GeoLocation> GetCoordinatesAsync(string city, CancellationToken cancellationToken);
}