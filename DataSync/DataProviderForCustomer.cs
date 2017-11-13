using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Entity;
using Newtonsoft.Json;

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

        public async Task<IList<Customer>> GetEntities()
        {
            var response = await WebApi.Get("api/TMSYX/BaseSync/GetCustomerTaxNumbers");
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
    }
}