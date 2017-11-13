using System.ServiceProcess;

namespace Service
{
    internal static class Program
    {
        /// <summary>
        ///     应用程序的主入口点。
        /// </summary>
        private static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new DataSyncService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}