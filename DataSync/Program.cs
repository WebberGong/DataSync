using System;
using System.IO;
using WindowsFirewallHelper;
using Common;
using log4net.Config;

namespace DataSync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));

            Starter.Start();

            var command = string.Empty;
            while (command != null && command.ToLower() != "exit")
            {
#if DEBUG
                LogHelper.LogInfo("输入 'exit' 然后按 Enter 键关闭");
#else
                Console.WriteLine("输入 'exit' 然后按 Enter 键关闭");
#endif
                command = Console.ReadLine();
            }
        }
    }
}