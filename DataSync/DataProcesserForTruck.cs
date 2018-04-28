using System.Threading.Tasks;
using Common;
using Entity;

namespace DataSync
{
    public class DataProcesserForTruck : IDataProcesser<Truck>
    {
        private static DataProcesserForTruck _instance;
        private static readonly object Locker = new object();

        private DataProcesserForTruck()
        {
        }

        public static DataProcesserForTruck Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProcesserForTruck();
                    }
                return _instance;
            }
        }

        public async Task<bool> Save(Truck entity)
        {
            return await WebApi.SimplePostAsync("api/TMSYX/BaseSync/SaveTruck", entity);
        }

        public async Task Sync(Truck entity)
        {
            await Relationships.AddCorpCars(entity);
        }
    }
}