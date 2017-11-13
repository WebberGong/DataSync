using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Entity;
using Newtonsoft.Json;

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

        public async Task<IList<Contract>> GetEntities()
        {
            var response = await WebApi.Get("api/TMSYX/BaseSync/GetContractNumbers");
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
    }
}