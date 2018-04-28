using System.Linq;
using System.Threading.Tasks;
using Common;
using Entity;

namespace DataSync
{
    public class DataProcesserForWeight : IDataProcesser<Weight>
    {
        private static DataProcesserForWeight _instance;
        private static readonly object Locker = new object();

        private DataProcesserForWeight()
        {
        }

        public static DataProcesserForWeight Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProcesserForWeight();
                    }
                return _instance;
            }
        }

        public async Task<bool> Save(Weight entity)
        {
            if (string.IsNullOrEmpty(entity.DispatchSn))
                using (var dbContext = new DatabaseContext<YxProcess>())
                {
                    var yxProcess = dbContext.Entities.IncludeAll().ToList()
                        .FirstOrDefault(x => x.WeightId == entity.Id);
                    if (yxProcess != null)
                    {
                        entity.DispatchSn = yxProcess.DispatchSn;
                        var cachedYxProcess =
                            DataSynchronizer<YxProcess>.SourceEntities.FirstOrDefault(x => x.Id == yxProcess.Id);
                        DataSynchronizer<YxProcess>.SourceEntities.Remove(cachedYxProcess);
                        DataSynchronizer<YxProcess>.SourceEntities.Add(yxProcess);
                    }
                    else
                    {
                        await LogHelper.LogErrorAsync("找不对应的运销过程数据");
                        return false;
                    }
                }

            if (string.IsNullOrEmpty(entity.DispatchSn))
            {
                await LogHelper.LogErrorAsync("派车单号不能为空");
                return false;
            }

            return await WebApi.SimplePostAsync(
                $"api/TMSYX/YXDispatch/WriteDispSendGross?dispatchSN={entity.DispatchSn}&" +
                $"truck_no={entity.TruckNumber}&pz={entity.Pz}&mz={entity.Mz}&jz={entity.Jz}", new object());
        }

        public async Task Sync(Weight entity)
        {
            await Task.Run(() => true);
        }
    }
}