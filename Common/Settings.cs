using System.Configuration;

namespace Common
{
    public class Settings
    {
        public static string ItmsBaseUrl => ConfigurationManager.AppSettings["ITMS.BaseUrl"];
    }
}