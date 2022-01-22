using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SWPCarAssistent.Infrastructure.Clients
{
    public class OpenWeatherApiHttpClient
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey;

        private Uri RequestUri { get; set; }

        public OpenWeatherApiHttpClient(string apiKey)
        {
            if(apiKey == null || apiKey.Length == 0)
                throw new ArgumentNullException(nameof(apiKey));

            httpClient = new HttpClient();
            this.apiKey = apiKey;
            PrepareRequestUri();
        }

        public async Task<QueryResponse> GetQueryAsync(string queryString)
        {
            var jsonResponse = await httpClient.GetStringAsync(RequestUri + queryString);
            
            if (jsonResponse is null || jsonResponse.Length == 0)
                throw new HttpRequestException();

            return new QueryResponse(jsonResponse);
        }

        private void PrepareRequestUri()
        {
            RequestUri = new Uri($"http://api.openweathermap.org/data/2.5/weather?appid=" + apiKey + "&q=");
        }
    }
}
