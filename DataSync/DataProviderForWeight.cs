using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;
using System.Linq.Expressions;
using System;
using System.Linq;

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

        public async Task<List<Weight>> GetEntities()
        {
            return await Task.Run(() => new List<Weight>());
        }

        public bool IsUpdateOnly => false;

        public bool IsForceInitSync => false;

        public Expression<Func<Weight, bool>> SynchronizationWhere()
        {
            return x => true;
        }

        public Expression<Func<Weight, bool>> UpdateWhere(IEnumerable<Weight> filter)
        {
            var keys = filter.Select(x => x.Id).ToList();
            return x => keys.Contains(x.Id);
        }

        public Expression<Func<Weight, bool>> DeleteWhere()
        {
            return x => false;
        }

        public List<Weight> GetPeriodicalSyncEntities()
        {
            using (var dbContext = new DatabaseContext<Weight>())
            {
                var haveUpdateTimeColumn = DataSynchronizer<Weight>.HaveUpdateTimeColumn();
                var lastSyncTime = Settings.GetLastSyncTime();
                var result = dbContext.Entities.IncludeAll().Where(x => (!haveUpdateTimeColumn || string.Compare(x.UpdateTime, lastSyncTime, StringComparison.CurrentCulture) >= 0)).ToList();
                return result;
            }
        }
    }
}