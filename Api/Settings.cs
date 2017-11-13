using System.Configuration;

namespace Api
{
    public class Settings
    {
        public static string ApiBaseUrl => ConfigurationManager.AppSettings["Api.BaseUrl"];
    }
}