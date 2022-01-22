using SWPCarAssistent.Core.Common.Models;

namespace SWPCarAssistent.Core.Common.Interfaces
{
    public interface IOpenWeatherMapService
    {
        WeatherRoot GetWeather(string city);
    }
}
