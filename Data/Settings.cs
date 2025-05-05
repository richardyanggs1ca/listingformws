using Microsoft.Extensions.Configuration;
namespace ListingFormWS.Data
{
    public class Settings
    {
        public string ConnectionString;
        public string Database;
        public string Certification;
        public string Host;
        public string Port;
        public string ContentPath;
        public IConfigurationRoot IConfigurationRoot;
    }
}