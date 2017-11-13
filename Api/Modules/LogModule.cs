using Common;
using Nancy;

namespace Api.Modules
{
    public class LogModule : NancyModule
    {
        public LogModule()
        {
            Get["api/log/", true] = async (x, ct) =>
            {
                var log = await LogHelper.ReadLog();
                return Response.AsText(log, "text/plain; charset=utf-8");
            };
        }
    }
}