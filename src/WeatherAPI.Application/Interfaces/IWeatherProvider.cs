using WeatherAPI.Domain.Enums;
using WeatherAPI.Domain.Models;

namespace WeatherAPI.Application.Interfaces;

public interface IWeatherProvider
{
    public Task<WeatherData> GetWeatherAsync(GeoLocation location, DateOnly dateOnly, CancellationToken cancellationToken);
    public Task<WeekForecast> GetWeekWeatherAsync(GeoLocation location, CancellationToken cancellationToken);
}