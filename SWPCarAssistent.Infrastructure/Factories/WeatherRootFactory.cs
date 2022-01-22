using Newtonsoft.Json.Linq;
using SWPCarAssistent.Core.Common.Exceptions;
using SWPCarAssistent.Core.Common.Models;
using System.Text.Json;

namespace SWPCarAssistent.Infrastructure.Factories
{
    public class WeatherRootFactory
    {
        public static WeatherRoot CreateObject(string json)
        {
            var jsonObject = JObject.Parse(json);

            if(jsonObject?.SelectToken("cod").ToString() == "200")
            {
                return JsonSerializer.Deserialize<WeatherRoot>(json);
            }

            throw new WeatherNotFoundException();
        }
    }
}
