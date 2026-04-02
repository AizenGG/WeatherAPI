namespace WeatherAPI.Domain.Models;

public class WeekForecast
{
    public string CityName { get; set; }
    public IReadOnlyList<WeatherData> Days { get; set; }
}