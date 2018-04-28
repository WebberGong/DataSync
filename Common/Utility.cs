using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Reflection;
using Entity;
using Entity.Attribute;
using Newtonsoft.Json;
using RestSharp;

namespace Common
{
    public class Utility
    {
        public static bool IsResponseSuccess(IRestResponse<WebApiResponse> response)
        {
            var baseMsg = $"调用 {response.Request.Method}: {response.Request.Resource} " +
                          $"参数: [{string.Join(", ", response.Request.Parameters.Select(x => $"{x.Name}:{x.Value}"))}]";
            if (response.StatusCode == HttpStatusCode.OK)
                switch (response.Data.Status)
                {
                    case 0:
                        LogHelper.LogError($"{baseMsg} 失败, 错误消息: {response.Data.Msg}");
                        return false;
                    case 1:
                        LogHelper.LogInfo($"{baseMsg} 成功, 返回数据: {JsonConvert.SerializeObject(response.Data)}");
                        return true;
                    default:
                        LogHelper.LogError($"{baseMsg} 失败, WebApi返回了超出预期的状态码: {response.Data.Status}");
                        return false;
                }
            LogFatalOrThrowException(response.ErrorException,
                $"{baseMsg} 失败, Http返回了错误的状态码: {response.StatusCode}, {response.Content}");
            return false;
        }

        public static void LogFatalOrThrowException(Exception ex, string msg = null)
        {
#if DEBUG
            throw ex;
#else
            LogHelper.LogFatal(msg ?? ex.Message, ex);
#endif
        }
    }
}