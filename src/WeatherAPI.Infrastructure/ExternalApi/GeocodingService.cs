using System.Text.Json;
using WeatherAPI.Application.Interfaces;
using WeatherAPI.Domain.Models;

namespace WeatherAPI.Infrastructure.ExternalApi;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GeoLocation> GetCoordinatesAsync(string city, CancellationToken cancellationToken)
    {
        var url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1&language=en";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var json = JsonDocument.Parse(content);
        var results = json.RootElement.GetProperty("results");
        if (results.GetArrayLength() == 0)
            throw new KeyNotFoundException($"City : {city}");
        var first = results[0];
        return new GeoLocation
        {
            Latitude = first.GetProperty("latitude").GetDouble(),
            Longitude = first.GetProperty("longitude").GetDouble(),
            DisplayName = first.GetProperty("name").GetString() ?? city
        };
    }
}