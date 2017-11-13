using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using log4net;

namespace Common
{
    public class LogHelper
    {
        public static void LogInfo(string info)
        {
            LogManager.GetLogger(GetTypeFromStackTrace()).Info(info);
            WriteRemoteLog(info);
        }

        public static void LogWarn(string warn)
        {
            LogManager.GetLogger(GetTypeFromStackTrace()).Warn(warn);
            WriteRemoteLog(warn);
        }

        public static void LogDebug(string debug)
        {
            LogManager.GetLogger(GetTypeFromStackTrace()).Debug(debug);
            WriteRemoteLog(debug);
        }

        public static void LogError(string error, Exception ex = null)
        {
            LogManager.GetLogger(GetTypeFromStackTrace()).Error(error, ex);
            WriteRemoteLog(error + "\r\n" + (ex == null ? string.Empty : ex.ToFormattedString()));
        }

        public static void LogFatal(string fatal, Exception ex = null)
        {
            LogManager.GetLogger(GetTypeFromStackTrace()).Fatal(fatal, ex);
            WriteRemoteLog(fatal + "\r\n" + (ex == null ? string.Empty : ex.ToFormattedString()));
        }

        public static async Task LogInfoAsync(string info)
        {
            LogManager.GetLogger(GetTypeFromStackTrace()).Info(info);
            await WriteRemoteLogAsync(info);
        }

        public static async Task LogWarnAsync(string warn)
        {
            LogManager.GetLogger(GetTypeFromStackTrace()).Warn(warn);
            await WriteRemoteLogAsync(warn);
        }

        public static async Task LogDebugAsync(string debug)
        {
            LogManager.GetLogger(GetTypeFromStackTrace()).Debug(debug);
            await WriteRemoteLogAsync(debug);
        }

        public static async Task LogErrorAsync(string error, Exception ex = null)
        {
            LogManager.GetLogger(GetTypeFromStackTrace()).Error(error, ex);
            await WriteRemoteLogAsync(error + "\r\n" + (ex == null ? string.Empty : ex.ToFormattedString()));
        }

        public static async Task LogFatalAsync(string fatal, Exception ex = null)
        {
            LogManager.GetLogger(GetTypeFromStackTrace()).Fatal(fatal, ex);
            await WriteRemoteLogAsync(fatal + "\r\n" + (ex == null ? string.Empty : ex.ToFormattedString()));
        }

        public static async Task<string> ReadLog()
        {
            var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log/DataSync.log"));
            return await reader.ReadToEndAsync();
        }

        private static Type GetTypeFromStackTrace()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(2);
            return sf?.GetMethod()?.DeclaringType;
        }

        private static void WriteRemoteLog(string log)
        {
            WebApi.Post("api/TMSYX/BaseSync/Log", log);
        }

        private static async Task WriteRemoteLogAsync(string log)
        {
            await WebApi.PostAsync("api/TMSYX/BaseSync/Log", log);
        }
    }
}