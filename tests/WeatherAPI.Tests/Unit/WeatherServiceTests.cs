using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherAPI.Application.Interfaces;
using WeatherAPI.Application.Services;
using WeatherAPI.Domain.Enums;
using WeatherAPI.Domain.Models;
using Xunit;

namespace WeatherAPI.Tests.Unit;

public class WeatherServiceTests
{
    private readonly Mock<IGeocodingService> _mockGeocoding;
    private readonly Mock<IWeatherProvider> _mockProvider;
    private readonly Mock<IRequestRepository> _mockRepository;
    private readonly IMemoryCache _cache;
    private readonly WeatherService _service;

    public WeatherServiceTests()
    {
        _mockGeocoding = new Mock<IGeocodingService>();
        _mockProvider = new Mock<IWeatherProvider>();
        _mockRepository = new Mock<IRequestRepository>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BaseUrl"] = "http://localhost:5000",
                ["Cache:TtlMinutes"] = "30"
            })
            .Build();

        var logger = new Mock<ILogger<WeatherService>>();

        _service = new WeatherService(
            _mockGeocoding.Object,
            _mockProvider.Object,
            _mockRepository.Object,
            _cache,
            logger.Object,
            config);
    }

    [Fact]
    public async Task GetWeatherDayAsync_CacheMiss()
    {
        var city = "London";
        var date = new DateOnly(2026, 3, 30);

        _mockGeocoding
            .Setup(x => x.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GeoLocation { Latitude = 51.5, Longitude = -0.12, DisplayName = "London" });

        _mockProvider
            .Setup(x => x.GetWeatherAsync(It.IsAny<GeoLocation>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeatherData { Date = date, Temperature = 12.1, Condition = WeatherCondition.Cloudy });

        _mockRepository
            .Setup(x => x.SaveAsync(It.IsAny<Domain.Entites.RequestLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var result = await _service.GetWeatherDayAsync(city, date, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal("London", result.City);
        Assert.Equal(12.1, result.TemperatureC);
        _mockGeocoding.Verify(x => x.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockProvider.Verify(x => x.GetWeatherAsync(It.IsAny<GeoLocation>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetWeatherDayAsync_CacheHit()
    {
        var city = "London";
        var date = new DateOnly(2026, 3, 30);

        _mockGeocoding
            .Setup(x => x.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GeoLocation { Latitude = 51.5, Longitude = -0.12, DisplayName = "London" });

        _mockProvider
            .Setup(x => x.GetWeatherAsync(It.IsAny<GeoLocation>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeatherData { Date = date, Temperature = 12.1, Condition = WeatherCondition.Cloudy });

        _mockRepository
            .Setup(x => x.SaveAsync(It.IsAny<Domain.Entites.RequestLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        await _service.GetWeatherDayAsync(city, date, CancellationToken.None);
        await _service.GetWeatherDayAsync(city, date, CancellationToken.None);
        _mockProvider.Verify(
            x => x.GetWeatherAsync(It.IsAny<GeoLocation>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _mockGeocoding.Verify(
            x => x.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
