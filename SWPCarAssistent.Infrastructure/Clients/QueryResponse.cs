using SWPCarAssistent.Core.Common.Models;
using SWPCarAssistent.Infrastructure.Factories;

namespace SWPCarAssistent.Infrastructure.Clients
{
    public class QueryResponse
    {
        public WeatherRoot WeatherRoot { get; }

        public QueryResponse(string jsonResponse)
        {
            WeatherRoot = WeatherRootFactory.CreateObject(jsonResponse);
        }
    }
}
