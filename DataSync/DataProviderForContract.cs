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
    public class DataProviderForContract : IDataProvider<Contract>
    {
        private static DataProviderForContract _instance;
        private static readonly object Locker = new object();

        private DataProviderForContract()
        {
        }

        public static DataProviderForContract Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProviderForContract();
                    }
                return _instance;
            }
        }

        public async Task<List<Contract>> GetEntities()
        {
            var response = await WebApi.GetAsync("api/TMSYX/BaseSync/GetContractNumbers");
            if (Utility.IsResponseSuccess(response))
            {
                var results = new List<Contract>();
                var arr = JsonConvert.DeserializeObject<string[]>(response.Data.Data);
                foreach (var item in arr)
                    results.Add(new Contract {Code = item});
                return results;
            }
            return new List<Contract>();
        }

        public bool IsUpdateOnly => false;

        public bool IsForceInitSync => false;

        public Expression<Func<Contract, bool>> SynchronizationWhere()
        {
            return x => x.SpState == 2;
        }

        public Expression<Func<Contract, bool>> UpdateWhere(IEnumerable<Contract> filter)
        {
            var keys = filter.Select(x => x.Code).ToList();
            return x => keys.Contains(x.Code);
        }

        public Expression<Func<Contract, bool>> DeleteWhere()
        {
            return x => false;
        }

        public List<Contract> GetPeriodicalSyncEntities()
        {
            using (var dbContext = new DatabaseContext<Contract>())
            {
                var haveUpdateTimeColumn = DataSynchronizer<Contract>.HaveUpdateTimeColumn();
                var lastSyncTime = Settings.GetLastSyncTime();
                var result = dbContext.Entities.IncludeAll().Where(x => (!haveUpdateTimeColumn || string.Compare(x.UpdateTime, lastSyncTime, StringComparison.CurrentCulture) >= 0)).ToList();
                return result;
            }
        }
    }
}