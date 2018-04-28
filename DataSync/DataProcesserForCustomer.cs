using System.Threading.Tasks;
using Common;
using Entity;
using System.Collections.Generic;

namespace DataSync
{
    public class DataProcesserForCustomer : IDataProcesser<Customer>
    {
        private static DataProcesserForCustomer _instance;
        private static readonly object Locker = new object();

        private DataProcesserForCustomer()
        {
        }

        public static DataProcesserForCustomer Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProcesserForCustomer();
                    }
                return _instance;
            }
        }

        public async Task<bool> Save(Customer entity)
        {
            return await WebApi.SimplePostAsync("api/TMSYX/BaseSync/SaveCustomer", entity);
        }

        public async Task Sync(Customer entity)
        {
            await Relationships.AddCooperationCorpForCustomer(entity);
        }
    }
}