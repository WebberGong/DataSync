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
            return await WebApi.SimplePostAsync("api/TMSYX/BaseSync/SaveCarrier", entity);
        }

        public async Task Sync(Carrier entity)
        {
            await Relationships.AddCooperationCorpForCarrier(entity);
        }
    }
}