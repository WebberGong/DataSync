using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Management;

namespace Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            var service = new ManagementObject($"Win32_Service.Name='{ServiceInstaller.ServiceName}'");
            var changeMethod = service.GetMethodParameters("Change");
            changeMethod["DesktopInteract"] = true;
            service.InvokeMethod("Change", changeMethod, null);
        }
    }
}