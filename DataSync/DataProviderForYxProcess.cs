using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;

namespace DataSync
{
    public class DataProviderForYxProcess : IDataProvider<YxProcess>
    {
        private static DataProviderForYxProcess _instance;
        private static readonly object Locker = new object();

        private DataProviderForYxProcess()
        {
        }

        public static DataProviderForYxProcess Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProviderForYxProcess();
                    }
                return _instance;
            }
        }

        public async Task<IList<YxProcess>> GetEntities()
        {
            return await Task.Run(() => new List<YxProcess>());
        }
    }
}