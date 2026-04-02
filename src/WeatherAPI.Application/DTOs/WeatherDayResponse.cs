namespace WeatherAPI.Application.DTOs;

public class WeatherDayResponse
{
    public string City {get; set;}
    public DateOnly Date { get; set; }
    public string Condition {get; set;}
    public double TemperatureC {get; set;}
    public string IconUrl {get; set;}
    public string Source {get; set;}
    public DateTime FetchedAt { get; set;}
}