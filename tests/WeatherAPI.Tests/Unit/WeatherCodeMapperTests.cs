using WeatherAPI.Application.Mapping;
using WeatherAPI.Domain.Enums;
using Xunit;

namespace WeatherAPI.Tests.Unit;

public class WeatherCodeMapperTests
{
    [Fact]
    public void FromCode_WhenCodeIs0_ReturnsClear()
    {
        var result = WeatherCodeMapper.FromCode(0);
        Assert.Equal(WeatherCondition.Clear, result);
    }
    [Fact]
    public void FromCode_WhenCodeIs3_ReturnsCloudy()
    {
        var result = WeatherCodeMapper.FromCode(3);
        Assert.Equal(WeatherCondition.Cloudy, result);
    }
    [Fact]
    public void FromCode_WhenCodeIs45_ReturnsFog()
    {
        var result = WeatherCodeMapper.FromCode(45);
        Assert.Equal(WeatherCondition.Fog, result);
    }
    [Fact]
    public void FromCode_WhenCodeIs61_ReturnsRain()
    {
        var result = WeatherCodeMapper.FromCode(61);
        Assert.Equal(WeatherCondition.Rain, result);
    }
    [Fact]
    public void FromCode_WhenCodeIs71_ReturnsSnow()
    {
        var result = WeatherCodeMapper.FromCode(71);
        Assert.Equal(WeatherCondition.Snow, result);
    }
    [Fact]
    public void FromCode_WhenCodeIs95_ReturnsThunder()
    {
        var result = WeatherCodeMapper.FromCode(95);
        Assert.Equal(WeatherCondition.Thunder, result);
    }
    [Fact]
    public void FromCode_WhenCodeIs999_ReturnsUnknown()
    {
        var result = WeatherCodeMapper.FromCode(999);
        Assert.Equal(WeatherCondition.Unknown, result);
    }
}