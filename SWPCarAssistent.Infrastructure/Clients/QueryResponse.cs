using SWPCarAssistent.Core.Common.Models;
using SWPCarAssistent.Infrastructure.Factories;

namespace SWPCarAssistent.Infrastructure.Clients
{
    public class QueryResponse
    {
        WeatherRoot WeatherRoot { get; set; }

        public QueryResponse(string jsonResponse)
        {
            WeatherRoot = WeatherRootFactory.CreateObject(jsonResponse);
        }
    }
}
