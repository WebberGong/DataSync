﻿using System;
using System.Configuration;
using Common;

namespace DataSync
{
    public class Settings
    {
        private const string LastSyncTimeKey = "LastSyncTime";

        public static string ConnectionString =>
            ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

        public static bool IsSyncAllData => GetBool("IsSyncAllData");

        public static bool IsSyncNewDataOnly => GetBool("IsSyncNewDataOnly");

        public static bool IsUpdateDateTime => GetBool("IsUpdateDateTime");

        public static bool IsPersistentNotification => GetBool("IsPersistentNotification");

        public static bool LazyLoadingEnabled => GetBool("LazyLoadingEnabled");

        public static int PeriodicalSynchronizeInterval => GetInt("PeriodicalSynchronizeInterval");

        public static DateTime LastSyncTime
        {
            get { return GetDateTime(LastSyncTimeKey); }
            set { SetDateTime(LastSyncTimeKey, value); }
        }

        public static string GetLastSyncTime()
        {
            //减去三秒偏移量
            return LastSyncTime.AddSeconds(-3).ToFormattedString();
        }

        private static DateTime GetDateTime(string key)
        {
            DateTime time;
            return DateTime.TryParse(ConfigurationManager.AppSettings[key], out time) ? time : DateTime.MinValue;
        }

        private static void SetDateTime(string key, DateTime value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] != null)
                config.AppSettings.Settings[key].Value = value.ToFormattedString();
            else
                config.AppSettings.Settings.Add(key, value.ToFormattedString());
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private static bool GetBool(string key)
        {
            bool value;
            return bool.TryParse(ConfigurationManager.AppSettings[key], out value) && value;
        }

        private static int GetInt(string key)
        {
            int value = 60;
            int.TryParse(ConfigurationManager.AppSettings[key], out value);
            return value;
        }
    }
}