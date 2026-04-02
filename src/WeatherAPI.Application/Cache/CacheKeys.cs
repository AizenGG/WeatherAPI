namespace WeatherAPI.Application.Cache;

public static class CacheKeys
{
    public static string ForDay(string city, DateOnly date) => $"weather:open-meteo:{city}:{date:yyyy-MM-dd}";
    public static string ForWeek(string city, DateOnly from) => $"weather-week:open-meteo:{city}:{from:yyyy-MM-dd}";
}