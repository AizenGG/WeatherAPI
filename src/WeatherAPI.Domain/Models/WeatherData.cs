using WeatherAPI.Domain.Enums;

namespace WeatherAPI.Domain.Models;

public class WeatherData
{
    public DateOnly Date { get; set; }
    public double Temperature { get; set; }
    public WeatherCondition Condition { get; set; }
}