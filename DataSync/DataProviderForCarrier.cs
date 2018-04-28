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
    public class DataProviderForCarrier : IDataProvider<Carrier>
    {
        private static DataProviderForCarrier _instance;
        private static readonly object Locker = new object();

        private DataProviderForCarrier()
        {
        }

        public static DataProviderForCarrier Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProviderForCarrier();
                    }
                return _instance;
            }
        }

        public async Task<List<Carrier>> GetEntities()
        {
            var response = await WebApi.GetAsync("api/TMSYX/BaseSync/GetCarrierTaxNumbers");
            if (Utility.IsResponseSuccess(response))
            {
                var results = new List<Carrier>();
                var arr = JsonConvert.DeserializeObject<string[]>(response.Data.Data);
                foreach (var item in arr)
                    results.Add(new Carrier {TaxNumber = item});
                return results;
            }
            return new List<Carrier>();
        }

        public bool IsUpdateOnly => false;

        public bool IsForceInitSync => true;

        public Expression<Func<Carrier, bool>> SynchronizationWhere()
        {
            return x => x.IsValid == 1;
        }

        public Expression<Func<Carrier, bool>> UpdateWhere(IEnumerable<Carrier> filter)
        {
            var keys = filter.Select(x => x.TaxNumber).ToList();
            return x => keys.Contains(x.TaxNumber);
        }

        public Expression<Func<Carrier, bool>> DeleteWhere()
        {
            return x => false;
        }

        public List<Carrier> GetPeriodicalSyncEntities()
        {
            using (var dbContext = new DatabaseContext<Carrier>())
            {
                var haveUpdateTimeColumn = DataSynchronizer<Carrier>.HaveUpdateTimeColumn();
                var lastSyncTime = Settings.GetLastSyncTime();
                var result = dbContext.Entities.IncludeAll().Where(x => (!haveUpdateTimeColumn || string.Compare(x.UpdateTime, lastSyncTime, StringComparison.CurrentCulture) >= 0)).ToList();
                return result;
            }
        }
    }
}