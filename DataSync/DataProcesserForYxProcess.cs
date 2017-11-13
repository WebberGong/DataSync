using System.Threading.Tasks;
using Common;
using Entity;

namespace DataSync
{
    public class DataProcesserForYxProcess : IDataProcesser<YxProcess>
    {
        private static DataProcesserForYxProcess _instance;
        private static readonly object Locker = new object();

        private DataProcesserForYxProcess()
        {
        }

        public static DataProcesserForYxProcess Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProcesserForYxProcess();
                    }
                return _instance;
            }
        }

        public async Task<bool> Save(YxProcess entity)
        {
            if (string.IsNullOrEmpty(entity.DispatchSn))
            {
                await LogHelper.LogErrorAsync("派车单号不能为空");
                return false;
            }

            var response = await WebApi.Post<BaseEntity>(
                $"api/TMSYX/YXDispatch/UpdateDispatchStatus?dispatchSN={entity.DispatchSn}&truck_no={entity.TruckNumber}&yxStatus={entity.YxStatus}");
            return Utility.IsResponseSuccess(response);
        }
    }
}