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
    public class DataProviderForSeller : IDataProvider<Seller>
    {
        private static DataProviderForSeller _instance;
        private static readonly object Locker = new object();

        private DataProviderForSeller()
        {
        }

        public static DataProviderForSeller Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProviderForSeller();
                    }
                return _instance;
            }
        }

        public async Task<List<Seller>> GetEntities()
        {
            var response = await WebApi.GetAsync("api/TMSYX/BaseSync/GetSellerTaxNumbers");
            if (Utility.IsResponseSuccess(response))
            {
                var results = new List<Seller>();
                var arr = JsonConvert.DeserializeObject<string[]>(response.Data.Data);
                foreach (var item in arr)
                    results.Add(new Seller {TaxNumber = item});
                return results;
            }
            return new List<Seller>();
        }

        public bool IsUpdateOnly => false;

        public bool IsForceInitSync => true;

        public Expression<Func<Seller, bool>> SynchronizationWhere()
        {
            return x => x.IsValid == 1;
        }

        public Expression<Func<Seller, bool>> UpdateWhere(IEnumerable<Seller> filter)
        {
            var keys = filter.Select(x => x.TaxNumber).ToList();
            return x => keys.Contains(x.TaxNumber);
        }

        public Expression<Func<Seller, bool>> DeleteWhere()
        {
            return x => false;
        }

        public List<Seller> GetPeriodicalSyncEntities()
        {
            using (var dbContext = new DatabaseContext<Seller>())
            {
                var haveUpdateTimeColumn = DataSynchronizer<Seller>.HaveUpdateTimeColumn();
                var lastSyncTime = Settings.GetLastSyncTime();
                var result = dbContext.Entities.IncludeAll().Where(x => (!haveUpdateTimeColumn || string.Compare(x.UpdateTime, lastSyncTime, StringComparison.CurrentCulture) >= 0)).ToList();
                return result;
            }
        }
    }
}