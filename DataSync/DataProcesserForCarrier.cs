using System.Threading.Tasks;
using Common;
using Entity;

namespace DataSync
{
    public class DataProcesserForCarrier : IDataProcesser<Carrier>
    {
        private static DataProcesserForCarrier _instance;
        private static readonly object Locker = new object();

        private DataProcesserForCarrier()
        {
        }

        public static DataProcesserForCarrier Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProcesserForCarrier();
                    }
                return _instance;
            }
        }

        public async Task<bool> Save(Carrier entity)
        {
            var response = await WebApi.Post("api/TMSYX/BaseSync/SaveCarrier", entity);
            return Utility.IsResponseSuccess(response);
        }
    }
}