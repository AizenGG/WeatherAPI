namespace WeatherAPI.Application.DTOs;

public class RequestLogDto
{
    public int Id {get; set;}
    public DateTime TimestampUtc {get; set;}
    public string Endpoint {get; set;}
    public string City {get; set;}
    public DateOnly? Date {get; set;}
    public bool CacheHit {get; set;}
    public int StatusCode {get; set;}
    public int LatencyMs {get; set;}
}