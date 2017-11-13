using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Entity;
using Newtonsoft.Json;

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

        public async Task<IList<Truck>> GetEntities()
        {
            var response = await WebApi.Get("api/TMSYX/BaseSync/GetTruckNumbers");
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
    }
}