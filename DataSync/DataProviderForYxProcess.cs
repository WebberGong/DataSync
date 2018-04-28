using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;
using Common;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace DataSync
{
    public class DataProviderForYxProcess : IDataProvider<YxProcess>
    {
        private static DataProviderForYxProcess _instance;
        private static readonly object Locker = new object();

        private DataProviderForYxProcess()
        {
        }

        public static DataProviderForYxProcess Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProviderForYxProcess();
                    }
                return _instance;
            }
        }

        public async Task<List<YxProcess>> GetEntities()
        {
            var response = await WebApi.GetAsync("api/TMSYX/BaseSync/GetDispatchProfiles");
            if (Utility.IsResponseSuccess(response))
            {
                var results = new List<YxProcess>();
                var arr = JsonConvert.DeserializeObject<string[]>(response.Data.Data);
                foreach (var item in arr)
                {
                    var parts = item.Split(new char[] { ',' });
                    if (string.IsNullOrEmpty(parts[0]))
                        continue;
                    var result = new YxProcess();
                    result.SetComparerKey(parts[0]);
                    var yxStatus = 0;
                    int.TryParse(parts[1], out yxStatus);
                    result.YxStatus = yxStatus;
                    var jz = 0d;
                    double.TryParse(parts[2], out jz);
                    var pz = 0d;
                    double.TryParse(parts[3], out pz);
                    result.Weight = new Weight
                    {
                        DispatchSn = parts[0],
                        Jz = jz,
                        Pz = pz
                    };
                    result.SendTruck = new SendTruck
                    {
                        DispatchSn = parts[0]
                    };
                    results.Add(result);
                }
                return results;
            }
            return new List<YxProcess>();
        }

        public bool IsUpdateOnly => true;

        public bool IsForceInitSync => false;

        public Expression<Func<YxProcess, bool>> SynchronizationWhere()
        {
            return x => true;
        }

        public Expression<Func<YxProcess, bool>> UpdateWhere(IEnumerable<YxProcess> filter)
        {
            var keys = filter.Select(x => x.DispatchSn).ToList();
            using (var dbContext = new DatabaseContext<SendTruck>())
            {
                var sendTruckIds = dbContext.Entities.Where(x => keys.Contains(x.DispatchSn)).Select(x => x.Id).ToList();
                return x => sendTruckIds.Contains(x.SendTruckId);
            }
        }

        public Expression<Func<YxProcess, bool>> DeleteWhere()
        {
            return x => x.YxStatus > 8;
        }

        public List<YxProcess> GetPeriodicalSyncEntities()
        {
            using (var dbContextYxProcess = new DatabaseContext<YxProcess>())
            {
                var haveUpdateTimeColumn = DataSynchronizer<YxProcess>.HaveUpdateTimeColumn();
                var lastSyncTime = Settings.GetLastSyncTime();
                var result = dbContextYxProcess.Entities.IncludeAll().Where(x => x.SendTruck.DispatchSn == null || x.YxStatus < 9 || (!haveUpdateTimeColumn || string.Compare(x.UpdateTime, lastSyncTime, StringComparison.CurrentCulture) >= 0)).ToList();
                return result;
            }
        }
    }
}