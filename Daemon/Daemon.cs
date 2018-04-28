using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Daemon
{
    public class Daemon
    {
        public static string ProcessName => ConfigurationManager.AppSettings["ProcessName"];

        public static string WorkingDirectory => ConfigurationManager.AppSettings["WorkingDirectory"];

        public static async void Start()
        {
            await Task.Run(() =>
            {
                AutoCall(StartDataSync);
            });
        }

        private static int GetInt(string key)
        {
            int value;
            int.TryParse(ConfigurationManager.AppSettings[key], out value);
            return value;
        }

        private static void StartDataSync()
        {
            var isProcessExisted = false;
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (process.ProcessName == ProcessName)
                {
                    isProcessExisted = true;
                    break;
                }
            }
            if (!isProcessExisted)
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = ProcessName + ".exe",
                        CreateNoWindow = false,
                        UseShellExecute = true,
                        WorkingDirectory = WorkingDirectory
                    }
                };
                process.Start();
            }
        }

        /// <summary>
        ///     自动调用
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        // ReSharper disable once FunctionRecursiveOnAllPaths
        private static void AutoCall(Action call)
        {
            try
            {
                call();
                ConfigurationManager.RefreshSection("appSettings");
                Thread.Sleep(1000 * GetInt("CallInteral"));
                LogHelper.LogInfo("运行数据同步程序守护进程正常", false);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("运行数据同步程序守护进程异常", ex, false);
            }
            finally
            {
                AutoCall(call);
            }
        }
    }
}
