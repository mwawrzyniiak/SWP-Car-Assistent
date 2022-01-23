using SWPCarAssistent.Core.Common.Interfaces;
using System.Configuration;

namespace SWPCarAssistent.Infrastructure.Configurations
{
    public class SimpleAppConfigurations : IAppConfigurations
    {
        public string API_KEY => ConfigurationManager.AppSettings["apiKey"];
    }
}
