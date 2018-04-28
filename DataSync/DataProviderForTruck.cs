using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Entity;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace DataSync
{
    public class DataProviderForTruck : IDataProvider<Truck>
    {
        private static DataProviderForTruck _instance;
        private static readonly object Locker = new object();

        private DataProviderForTruck()
        {
        }

        public static DataProviderForTruck Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProviderForTruck();
                    }
                return _instance;
            }
        }

        public async Task<List<Truck>> GetEntities()
        {
            var response = await WebApi.GetAsync("api/TMSYX/BaseSync/GetTruckNumbers");
            if (Utility.IsResponseSuccess(response))
            {
                var results = new List<Truck>();
                var arr = JsonConvert.DeserializeObject<string[]>(response.Data.Data);
                foreach (var item in arr)
                    results.Add(new Truck {Number = item});
                return results;
            }
            return new List<Truck>();
        }

        public bool IsUpdateOnly => false;

        public bool IsForceInitSync => false;

        public Expression<Func<Truck, bool>> SynchronizationWhere()
        {
            return x => x.IsValid == 1;
        }

        public Expression<Func<Truck, bool>> UpdateWhere(IEnumerable<Truck> filter)
        {
            var keys = filter.Select(x => x.Number).ToList();
            return x => keys.Contains(x.Number);
        }

        public Expression<Func<Truck, bool>> DeleteWhere()
        {
            return x => false;
        }

        public List<Truck> GetPeriodicalSyncEntities()
        {
            using (var dbContext = new DatabaseContext<Truck>())
            {
                var haveUpdateTimeColumn = DataSynchronizer<Truck>.HaveUpdateTimeColumn();
                var lastSyncTime = Settings.GetLastSyncTime();
                var result = dbContext.Entities.IncludeAll().Where(x => (!haveUpdateTimeColumn || string.Compare(x.UpdateTime, lastSyncTime, StringComparison.CurrentCulture) >= 0)).ToList();
                return result;
            }
        }
    }
}