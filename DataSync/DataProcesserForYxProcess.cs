using System.Linq;
using System.Threading.Tasks;
using Common;
using Entity;
using RestSharp;

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
                using (var dbContext = new DatabaseContext<SendTruck>())
                {
                    var sendTruck = dbContext.Entities.IncludeAll().Where(x => x.Id == entity.SendTruckId).ToList().FirstOrDefault();
                    if (sendTruck == null)
                    {
                        await LogHelper.LogErrorAsync("找不到对应的派车信息");
                        return false;
                    }
                    var response = await WebApi.PostAsync(
                        $"api/TMSYX/YXDispatch/SyncDispatchInfo?contractCode={sendTruck.Contract.Code}&" +
                        $"taxNumber={sendTruck.Carrier.TaxNumber}&truck_no={entity.TruckNumber}", new object());
                    var isSuccess = Utility.IsResponseSuccess(response);
                    if (isSuccess)
                    {
                        sendTruck.DispatchSn = response.Data.Data;
                        dbContext.SaveChanges();
                    }
                }
            }

            var response1 = await WebApi.PostAsync(
                $"api/TMSYX/YXDispatch/UpdateDispatchStatus?dispatchSN={entity.DispatchSn}&truck_no={entity.TruckNumber}&yxStatus={entity.YxStatus}", new object());
            IRestResponse<WebApiResponse> response2 = null;
            if (entity.Weight != null && (entity.Weight.Jz > 0 || entity.Weight.Pz > 0))
            {
                await Task.Delay(1000);
                response2 = await WebApi.PostAsync(
                    $"api/TMSYX/YXDispatch/WriteDispSendGross?dispatchSN={entity.DispatchSn}&" +
                    $"truck_no={entity.TruckNumber}&pz={entity.Weight.Pz}&mz={entity.Weight.Mz}&jz={entity.Weight.Jz}", new object());
            }
            return Utility.IsResponseSuccess(response1) && (response2 == null || Utility.IsResponseSuccess(response2));
        }

        public async Task Sync(YxProcess entity)
        {
            await Task.Run(() => true);
        }
    }
}