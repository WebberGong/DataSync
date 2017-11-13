using System.Threading.Tasks;
using Common;
using Entity;

namespace DataSync
{
    public class DataProcesserForSeller : IDataProcesser<Seller>
    {
        private static DataProcesserForSeller _instance;
        private static readonly object Locker = new object();

        private DataProcesserForSeller()
        {
        }

        public static DataProcesserForSeller Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProcesserForSeller();
                    }
                return _instance;
            }
        }

        public async Task<bool> Save(Seller entity)
        {
            var response = await WebApi.Post("api/TMSYX/BaseSync/SaveSeller", entity);
            return Utility.IsResponseSuccess(response);
        }
    }
}