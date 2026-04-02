using System.Text.Json;
using System.Text.Json.Nodes;
using System.Globalization;
using Microsoft.Extensions.Logging;
using WeatherAPI.Application.Interfaces;
using WeatherAPI.Domain.Models;
using WeatherAPI.Application.Mapping;

namespace WeatherAPI.Infrastructure.ExternalApi;

public class OpenMeteoClient : IWeatherProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenMeteoClient> _logger;

    public OpenMeteoClient(HttpClient httpClient, ILogger<OpenMeteoClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<WeatherData> GetWeatherAsync(GeoLocation location, DateOnly dateOnly, CancellationToken cancellationToken)
    {
        var dateStr = dateOnly.ToString("yyyy-MM-dd");
        var url = $"https://api.open-meteo.com/v1/forecast" +
                  $"?latitude={location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                  $"&longitude={location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                  $"&daily=weather_code,temperature_2m_max" +
                  $"&start_date={dateStr}&end_date={dateStr}&timezone=UTC";
        _logger.LogInformation("Requesting URL: {Url}", url);
        var response = await _httpClient.GetAsync(url,  cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<OpenMeteoResponse>(content) ?? throw new InvalidOperationException("Bad data from open-meteo");
        var code = data.Daily.WeatherCode[0];
        var temp = data.Daily.TemperatureMax[0];
        return new WeatherData
        {
            Date = dateOnly,
            Temperature = temp,
            Condition = WeatherCodeMapper.FromCode(code)
        };

    }

    public async Task<WeekForecast> GetWeekWeatherAsync(GeoLocation location, CancellationToken cancellationToken)
    {
        var url = $"https://api.open-meteo.com/v1/forecast" +
                  $"?latitude={location.Latitude.ToString(CultureInfo.InvariantCulture)}" +
                  $"&longitude={location.Longitude.ToString(CultureInfo.InvariantCulture)}" +
                  $"&daily=weather_code,temperature_2m_max" +
                  $"&forecast_days=7&timezone=UTC";
        _logger.LogInformation("Requesting URL: {Url}", url);
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<OpenMeteoResponse>(content);
        var days = data.Daily.Time
            .Select((t, i) => new WeatherData
            {
                Date = DateOnly.Parse(t),
                Temperature = data.Daily.TemperatureMax[i],
                Condition = WeatherCodeMapper.FromCode(data.Daily.WeatherCode[i])
            }).ToList();
        return new WeekForecast
        {
            CityName = location.DisplayName,
            Days = days
        };
    }
}