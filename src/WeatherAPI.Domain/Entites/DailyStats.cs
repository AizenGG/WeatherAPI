namespace WeatherAPI.Domain.Entites;

public class DailyStats
{
    public DateOnly Date { get; set; }
    public string City { get; set; } = string.Empty;
    public int Count { get; set; }
}