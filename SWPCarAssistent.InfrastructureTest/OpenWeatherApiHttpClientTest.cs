using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWPCarAssistent.Infrastructure.Clients;
using System.Threading.Tasks;

namespace SWPCarAssistent.InfrastructureTest
{
    [TestClass]
    public class OpenWeatherApiHttpClientTest
    {
        private readonly OpenWeatherApiHttpClient openWeatherApiHttpClient;
        private readonly string apiKey = ""; //ADD HERE YOUR API-KEY

        public OpenWeatherApiHttpClientTest()
        {
            openWeatherApiHttpClient = new OpenWeatherApiHttpClient(apiKey);
        }

        [TestMethod]
        public async Task GetQueryAsyncTest()
        {
            var test = openWeatherApiHttpClient.GetQueryAsync("Warszawa");

            var result = test.Result;
            Assert.AreEqual(result.WeatherRoot.name, "Warsaw");
        }
    }
}
