using System.IO;
using System.Reflection;
using System.ServiceProcess;
using WindowsFirewallHelper;
using DataSync;
using log4net.Config;

namespace Service
{
    public partial class DataSyncService : ServiceBase
    {
        public DataSyncService()
        {
            InitializeComponent();

            var assemblyFilePath = Assembly.GetExecutingAssembly().Location;
            var assemblyDirPath = Path.GetDirectoryName(assemblyFilePath);
            if (assemblyDirPath != null)
            {
                var configFilePath = Path.Combine(assemblyDirPath, "log4net.config");
                XmlConfigurator.ConfigureAndWatch(new FileInfo(configFilePath));
            }
        }

        protected override void OnStart(string[] args)
        {
            if (Starter.AddFirewallRule("Data Sync Inbound Rule", "Service.exe",
                    FirewallDirection.Inbound) &&
                Starter.AddFirewallRule("Data Sync Outbound Rule", "Service.exe",
                    FirewallDirection.Outbound))
                Starter.Start();
        }
    }
}