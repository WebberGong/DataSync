using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WindowsFirewallHelper;
using Api;
using Common;
using Entity;

namespace DataSync
{
    public class Starter
    {
        public static void Start()
        {
            WriteSplitLine();

            LogHelper.LogInfo("数据同步程序正在运行中...");

            WriteSplitLine();

            LogHelper.LogInfo("开始启动日志查看WebApi");
            SelfHost.Start();
            LogHelper.LogInfo($"启动日志查看WebApi: '{new Uri($"{Api.Settings.ApiBaseUrl}")}', 完成");

            WriteSplitLine();

            LogHelper.LogInfo("开始初始化数据");
            InitData();
            LogHelper.LogInfo("初始化数据完成");

            WriteSplitLine();

            LogHelper.LogInfo("开始同步数据");
            Synchronize();
            LogHelper.LogInfo("同步数据完成");

            WriteSplitLine();

            LogHelper.LogInfo("开始添加合作公司");
            Relationships.AddCooperationCorp();
            LogHelper.LogInfo("添加合作公司完成");

            WriteSplitLine();

            LogHelper.LogInfo("开始添加公司和车辆关系");
            Relationships.AddCorpCars();
            LogHelper.LogInfo("添加公司和车辆关系完成");

            WriteSplitLine();

            LogHelper.LogInfo("开始添加数据修改通知");
            AddNotification();
            LogHelper.LogInfo("添加数据修改通知完成");

            WriteSplitLine();

            PeriodicalSynchronize();
        }

        public static bool AddFirewallRule(string ruleName, string programName,
            FirewallDirection direction)
        {
            WriteSplitLine();
            LogHelper.LogInfo($"开始添加防火墙规则: '{ruleName}'");
            try
            {
                var existRule = FirewallManager.Instance.Rules.FirstOrDefault(x => x.Name == ruleName);
                if (existRule != null)
                    FirewallManager.Instance.Rules.Remove(existRule);
                var rule = FirewallManager.Instance.CreateApplicationRule(
                    FirewallManager.Instance.GetProfile().Type, ruleName,
                    FirewallAction.Allow, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, programName));
                rule.Direction = direction;
                FirewallManager.Instance.Rules.Add(rule);
                LogHelper.LogInfo($"添加防火墙规则: '{ruleName}', 完成");
                return true;
            }
            catch (Exception ex)
            {
                Utility.LogFatalOrThrowException(ex, $"添加防火墙规则: '{ruleName}', 失败");
                return false;
            }
        }

        private static void WriteSplitLine()
        {
            LogHelper.LogInfo(new string('-', 50));
        }

        private static void InitData()
        {
            try
            {
                var now = DateTime.Now;
                Task[] tasks =
                {
                    DataSynchronizer<Customer>.InitData(DataProviderForCustomer.Instance),
                    DataSynchronizer<Seller>.InitData(DataProviderForSeller.Instance),
                    DataSynchronizer<Carrier>.InitData(DataProviderForCarrier.Instance),
                    DataSynchronizer<Contract>.InitData(DataProviderForContract.Instance),
                    DataSynchronizer<Truck>.InitData(DataProviderForTruck.Instance),
                    DataSynchronizer<Weight>.InitData(DataProviderForWeight.Instance),
                    DataSynchronizer<YxProcess>.InitData(DataProviderForYxProcess.Instance)
                };
                Task.WaitAll(tasks);
                Settings.LastSyncTime = now;
            }
            catch (AggregateException ex)
            {
                Utility.LogFatalOrThrowException(ex);
            }
        }

        private static void Synchronize()
        {
            try
            {
                Task[] tasks =
                {
                    DataSynchronizer<Customer>.Synchronize(DataProcesserForCustomer.Instance,
                        DataProviderForCustomer.Instance),
                    DataSynchronizer<Seller>.Synchronize(DataProcesserForSeller.Instance,
                        DataProviderForSeller.Instance),
                    DataSynchronizer<Carrier>.Synchronize(DataProcesserForCarrier.Instance,
                        DataProviderForCarrier.Instance),
                    DataSynchronizer<Contract>.Synchronize(DataProcesserForContract.Instance,
                        DataProviderForContract.Instance),
                    DataSynchronizer<Truck>.Synchronize(DataProcesserForTruck.Instance,
                        DataProviderForTruck.Instance),
                    DataSynchronizer<YxProcess>.Synchronize(DataProcesserForYxProcess.Instance,
                        DataProviderForYxProcess.Instance)
                };
                Task.WaitAll(tasks);
            }
            catch (AggregateException ex)
            {
                Utility.LogFatalOrThrowException(ex);
            }
        }

        private static void AddNotification()
        {
            try
            {
                Task[] tasks =
                {
                    DataNotifier<Customer>.AddNotification(DataProcesserForCustomer.Instance,
                        DataProviderForCustomer.Instance),
                    DataNotifier<Seller>.AddNotification(DataProcesserForSeller.Instance,
                        DataProviderForSeller.Instance),
                    DataNotifier<Carrier>.AddNotification(DataProcesserForCarrier.Instance,
                        DataProviderForCarrier.Instance),
                    DataNotifier<Contract>.AddNotification(DataProcesserForContract.Instance,
                        DataProviderForContract.Instance),
                    DataNotifier<Truck>.AddNotification(DataProcesserForTruck.Instance,
                        DataProviderForTruck.Instance),
                    DataNotifier<Weight>.AddNotification(DataProcesserForWeight.Instance,
                        DataProviderForWeight.Instance),
                    DataNotifier<YxProcess>.AddNotification(DataProcesserForYxProcess.Instance,
                        DataProviderForYxProcess.Instance)
                };
                Task.WaitAll(tasks);
            }
            catch (AggregateException ex)
            {
                Utility.LogFatalOrThrowException(ex);
            }
        }

        private static async void PeriodicalSynchronize()
        {
            try
            {
                var now = DateTime.Now;
                LogHelper.LogInfo("开始定时同步数据");
                Task[] tasks =
                {
                    DataSynchronizer<Customer>.PeriodicalSynchronize(DataProcesserForCustomer.Instance,
                        DataProviderForCustomer.Instance),
                    DataSynchronizer<Seller>.PeriodicalSynchronize(DataProcesserForSeller.Instance,
                        DataProviderForSeller.Instance),
                    DataSynchronizer<Carrier>.PeriodicalSynchronize(DataProcesserForCarrier.Instance,
                        DataProviderForCarrier.Instance),
                    DataSynchronizer<Contract>.PeriodicalSynchronize(DataProcesserForContract.Instance,
                        DataProviderForContract.Instance),
                    DataSynchronizer<Truck>.PeriodicalSynchronize(DataProcesserForTruck.Instance,
                        DataProviderForTruck.Instance),
                    DataSynchronizer<YxProcess>.PeriodicalSynchronize(DataProcesserForYxProcess.Instance,
                        DataProviderForYxProcess.Instance)
                };
                await Task.WhenAll(tasks);
                Settings.LastSyncTime = now;
                LogHelper.LogInfo("定时同步数据完成");
                WriteSplitLine();
                await Task.Delay(Settings.PeriodicalSynchronizeInterval * 1000);
                PeriodicalSynchronize();
            }
            catch (AggregateException ex)
            {
                Utility.LogFatalOrThrowException(ex);
            }
        }
    }
}