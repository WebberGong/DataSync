using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Entity;
using Newtonsoft.Json;

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

        public async Task<IList<Carrier>> GetEntities()
        {
            var response = await WebApi.Get("api/TMSYX/BaseSync/GetCarrierTaxNumbers");
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
    }
}