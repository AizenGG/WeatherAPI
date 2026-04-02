using System.Text.Json.Serialization;

namespace WeatherAPI.Infrastructure.ExternalApi;

public class OpenMeteoResponse
{
    [JsonPropertyName("daily")] 
    public DailyData Daily { get; set; } = new();
}

public class DailyData
{
    [JsonPropertyName("time")] 
    public List<string> Time { get; set; } = new();
    
    [JsonPropertyName("weather_code")]
    public List<int> WeatherCode { get; set; } = new();

    [JsonPropertyName("temperature_2m_max")]
    public List<double> TemperatureMax { get; set; } = new();
}