using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWPCarAssistent.Infrastructure.Factories;

namespace SWPCarAssistent.InfrastructureTest
{
    [TestClass]
    public class WeatherRootFactoryTests
    {
        [TestMethod]
        public void CreateObject()
        {
            var testJson = System.Text.Encoding.UTF8.GetString(Properties.Resource.ExampleGetFromApi);

            var weatherRootObject = WeatherRootFactory.CreateObject(testJson);

            Assert.AreEqual(weatherRootObject.name, "Warsaw");
        }
    }
}
