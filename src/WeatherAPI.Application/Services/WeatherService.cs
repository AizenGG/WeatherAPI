using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WeatherAPI.Application.DTOs;
using Microsoft.Extensions.Configuration;
using WeatherAPI.Application.Interfaces;
using WeatherAPI.Domain.Entites;
using WeatherAPI.Domain.Enums;
using WeatherAPI.Application.Cache;
using WeatherAPI.Application.Mapping;

namespace WeatherAPI.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly IGeocodingService _geocodingService;
    private readonly IWeatherProvider _weatherProvider;
    private readonly IRequestRepository _repository;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<WeatherService> _logger;
    private readonly string _baseUrl;
    private readonly int _cacheTtlMinutes;

    public WeatherService(IGeocodingService geocodingService,  IWeatherProvider weatherProvider, IRequestRepository requestRepository, IMemoryCache memoryCache, ILogger<WeatherService> logger, IConfiguration config)
    {
        _geocodingService = geocodingService;
        _weatherProvider = weatherProvider;
        _repository = requestRepository;
        _memoryCache = memoryCache;
        _logger = logger;
        _baseUrl = config["BaseUrl"] ?? "http://localhost:5000";
        _cacheTtlMinutes = int.Parse(config["Cache:TtlMinutes"] ?? "30");
    }
    
    public async Task<WeatherDayResponse> GetWeatherDayAsync(string city, DateOnly date, CancellationToken cancellationToken)
    {
        var cityNorm = city.Trim().ToLower();
        var cacheKey = CacheKeys.ForDay(cityNorm, date);
        if (_memoryCache.TryGetValue(cacheKey, out WeatherDayResponse? cachedWeatherDay))
        {
            _logger.LogInformation("Cache hit for {City} Date {Date}", cityNorm, date);
            await SaveStatAsync(cityNorm, null, EndpointType.Day, cacheHit: true, statusCode: 200, latencyMs: 0,
                cancellationToken);
            return cachedWeatherDay!;
        }
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Fetching day weather for {City}", cityNorm);
        var location = await _geocodingService.GetCoordinatesAsync(city, cancellationToken);
        var weatherData = await _weatherProvider.GetWeatherAsync(location, date, cancellationToken);
        sw.Stop();
        var iconFile = WeatherCodeMapper.ToIconFileName(weatherData.Condition);
        var response = new WeatherDayResponse
        {
            City = location.DisplayName,
            Date = date,
            Condition = weatherData.Condition.ToString().ToLower(),
            TemperatureC = weatherData.Temperature,
            IconUrl = $"{_baseUrl}/static/icons/{iconFile}",
            Source = "open-meteo",
            FetchedAt = DateTime.UtcNow
        };
        _memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(_cacheTtlMinutes));
        await SaveStatAsync(cityNorm, date, EndpointType.Day, cacheHit: false, statusCode: 200,
            latencyMs: (int)sw.ElapsedMilliseconds, cancellationToken);
        return response;
    }

    public async Task<WeatherWeekResponse> GetWeatherWeekAsync(string city, CancellationToken cancellationToken)
    {
        var cityNorm = city.Trim().ToLower();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var cacheKey = CacheKeys.ForWeek(cityNorm, today);
        if (_memoryCache.TryGetValue(cacheKey, out WeatherWeekResponse? cachedWeatherWeek))
        {
            _logger.LogInformation("Cache hit for {City} Date {Today}", cityNorm,  today);
            await SaveStatAsync(cityNorm, null, EndpointType.Week, cacheHit: true, statusCode: 200, latencyMs: 0,
                cancellationToken);
            return cachedWeatherWeek!;
        }
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Fetching week weather for {City}", cityNorm);
        var location = await _geocodingService.GetCoordinatesAsync(city, cancellationToken);
        var forecast = await _weatherProvider.GetWeekWeatherAsync(location, cancellationToken);
        sw.Stop();
        var days = forecast.Days.Select(d =>
        {
            var icon = WeatherCodeMapper.ToIconFileName(d.Condition);
            return new WeatherDayEntry
            {
                Date = d.Date,
                Condition = d.Condition.ToString().ToLower(),
                IconUrl = $"{_baseUrl}/static/icons/{icon}",
                TemperatureC = d.Temperature
            };
        }).ToList();
        var response = new WeatherWeekResponse
        {
            City = location.DisplayName,
            Days = days,
            FetchedAt = DateTime.UtcNow,
            Source = "open-meteo"
        };
        _memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(_cacheTtlMinutes));
        await SaveStatAsync(cityNorm, null, EndpointType.Week, cacheHit: false, statusCode: 200, latencyMs: (int)sw.ElapsedMilliseconds, cancellationToken);
        return response;
    }
    private async Task SaveStatAsync(
        string city, DateOnly? date, EndpointType endpoint,
        bool cacheHit, int statusCode, int latencyMs,
        CancellationToken cancellationToken)
    {
        var log = new RequestLog
        {
            TimestampUtc = DateTime.UtcNow,
            City         = city,
            Date         = date,
            Endpoint     = endpoint.ToString(),
            CacheHit     = cacheHit,
            StatusCode   = statusCode,
            LatencyMs    = latencyMs
        };
        await _repository.SaveAsync(log, cancellationToken);
    }
}