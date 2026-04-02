using WeatherAPI.Application.DTOs;
using WeatherAPI.Domain.Enums;

namespace WeatherAPI.Application.Interfaces;

public interface IWeatherService
{
    public Task<WeatherDayResponse> GetWeatherDayAsync(string city, DateOnly date, CancellationToken cancellationToken);
    public Task<WeatherWeekResponse> GetWeatherWeekAsync(string city, CancellationToken cancellationToken); 
}