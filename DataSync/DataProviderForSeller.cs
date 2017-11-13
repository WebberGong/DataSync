using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Entity;
using Newtonsoft.Json;

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

        public async Task<IList<Seller>> GetEntities()
        {
            var response = await WebApi.Get("api/TMSYX/BaseSync/GetSellerTaxNumbers");
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
    }
}