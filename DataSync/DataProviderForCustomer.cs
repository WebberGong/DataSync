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
    public class DataProviderForCustomer : IDataProvider<Customer>
    {
        private static DataProviderForCustomer _instance;
        private static readonly object Locker = new object();

        private DataProviderForCustomer()
        {
        }

        public static DataProviderForCustomer Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProviderForCustomer();
                    }
                return _instance;
            }
        }

        public async Task<List<Customer>> GetEntities()
        {
            var response = await WebApi.GetAsync("api/TMSYX/BaseSync/GetCustomerTaxNumbers");
            if (Utility.IsResponseSuccess(response))
            {
                var results = new List<Customer>();
                var arr = JsonConvert.DeserializeObject<string[]>(response.Data.Data);
                foreach (var item in arr)
                    results.Add(new Customer {TaxNumber = item});
                return results;
            }
            return new List<Customer>();
        }

        public bool IsUpdateOnly => false;

        public bool IsForceInitSync => true;

        public Expression<Func<Customer, bool>> SynchronizationWhere()
        {
            return x => x.IsValid == 1;
        }

        public Expression<Func<Customer, bool>> UpdateWhere(IEnumerable<Customer> filter)
        {
            var keys = filter.Select(x => x.TaxNumber).ToList();
            return x => keys.Contains(x.TaxNumber);
        }

        public Expression<Func<Customer, bool>> DeleteWhere()
        {
            return x => false;
        }

        public List<Customer> GetPeriodicalSyncEntities()
        {
            using (var dbContext = new DatabaseContext<Customer>())
            {
                var haveUpdateTimeColumn = DataSynchronizer<Customer>.HaveUpdateTimeColumn();
                var lastSyncTime = Settings.GetLastSyncTime();
                var result = dbContext.Entities.IncludeAll().Where(x => (!haveUpdateTimeColumn || string.Compare(x.UpdateTime, lastSyncTime, StringComparison.CurrentCulture) >= 0)).ToList();
                return result;
            }
        }
    }
}