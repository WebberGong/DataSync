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
            return await WebApi.SimplePostAsync("api/TMSYX/BaseSync/SaveSeller", entity);
        }

        public async Task Sync(Seller entity)
        {
            await Relationships.AddCooperationCorpForSeller(entity);
        }
    }
}