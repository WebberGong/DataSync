using System.Configuration;

namespace Entity
{
    public class Settings
    {
        public static string DatabaseSchema => ConfigurationManager.AppSettings["DatabaseSchema"];
    }
}