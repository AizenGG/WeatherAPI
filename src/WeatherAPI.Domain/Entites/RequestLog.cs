using WeatherAPI.Domain.Enums;

namespace WeatherAPI.Domain.Entites;

public class RequestLog
{
    public int Id { get; set; }
    public DateTime TimestampUtc { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateOnly? Date { get; set; }
    public bool CacheHit  { get; set; }
    public int StatusCode { get; set; }
    public int LatencyMs { get; set; }
}