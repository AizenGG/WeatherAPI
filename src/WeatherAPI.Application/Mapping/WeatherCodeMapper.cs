using WeatherAPI.Domain.Enums;

namespace WeatherAPI.Application.Mapping;

public static class WeatherCodeMapper
{
    private static readonly Dictionary<int, WeatherCondition> _map = new()
    {
        [0]  = WeatherCondition.Clear,
        [1]  = WeatherCondition.Cloudy,
        [2]  = WeatherCondition.Cloudy,
        [3]  = WeatherCondition.Cloudy,
        [45] = WeatherCondition.Fog,
        [48] = WeatherCondition.Fog,
        [51] = WeatherCondition.Rain,
        [53] = WeatherCondition.Rain,
        [55] = WeatherCondition.Rain,
        [56] = WeatherCondition.FreezingRain,
        [57] = WeatherCondition.FreezingRain,
        [61] = WeatherCondition.Rain,
        [63] = WeatherCondition.Rain,
        [65] = WeatherCondition.Rain,
        [66] = WeatherCondition.FreezingRain,
        [67] = WeatherCondition.FreezingRain,
        [71] = WeatherCondition.Snow,
        [73] = WeatherCondition.Snow,
        [75] = WeatherCondition.Snow,
        [77] = WeatherCondition.Snow,
        [80] = WeatherCondition.Rain,
        [81] = WeatherCondition.Rain,
        [82] = WeatherCondition.Rain,
        [85] = WeatherCondition.Snow,
        [86] = WeatherCondition.Snow,
        [95] = WeatherCondition.Thunder,
        [96] = WeatherCondition.Thunder,
        [99] = WeatherCondition.Thunder,
    };
    
    public static WeatherCondition FromCode(int code)
    {
        return  _map.TryGetValue(code, out WeatherCondition condition) ?  condition : WeatherCondition.Unknown;
    }

    public static string ToIconFileName(WeatherCondition condition) =>
        condition switch
        {
            WeatherCondition.Clear        => "clear.png",
            WeatherCondition.Cloudy       => "cloudy.png",
            WeatherCondition.Fog          => "fog.png",
            WeatherCondition.Rain         => "rain.png",
            WeatherCondition.FreezingRain => "freezing-rain.png",
            WeatherCondition.Snow         => "snow.png",
            WeatherCondition.Thunder      => "thunder.png",
            WeatherCondition.Unknown      => "unknown.png",
            _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
        };
}