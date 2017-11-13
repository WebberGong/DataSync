using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;

namespace DataSync
{
    public class DataProviderForWeight : IDataProvider<Weight>
    {
        private static DataProviderForWeight _instance;
        private static readonly object Locker = new object();

        private DataProviderForWeight()
        {
        }

        public static DataProviderForWeight Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProviderForWeight();
                    }
                return _instance;
            }
        }

        public async Task<IList<Weight>> GetEntities()
        {
            return await Task.Run(() => new List<Weight>());
        }
    }
}