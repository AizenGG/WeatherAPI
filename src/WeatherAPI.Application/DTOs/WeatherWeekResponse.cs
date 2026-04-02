namespace WeatherAPI.Application.DTOs;

public class WeatherWeekResponse
{
   public string City {get; set;}
   public IReadOnlyList<WeatherDayEntry> Days {get; set;}
   public string Source {get; set;}
   public DateTime FetchedAt {get; set;}
}

public class WeatherDayEntry
{
   public DateOnly Date { get; set; }
   public string Condition { get; set; }
   public double TemperatureC{ get; set; }
   public string IconUrl { get; set; }
}
