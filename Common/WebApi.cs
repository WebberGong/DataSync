using System.Threading.Tasks;
using Entity;
using Newtonsoft.Json;
using RestSharp;

namespace Common
{
    public class WebApi
    {
        public static async Task<IRestResponse<WebApiResponse>> Post<TEntity>(string url, TEntity entity = null)
            where TEntity : BaseEntity
        {
            var client = new RestClient(Settings.ItmsBaseUrl);
            var request = new RestRequest(url, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("UID", "123456");
            request.AddHeader("Token", "abcdef");
            request.AddParameter("application/json", JsonConvert.SerializeObject(entity), ParameterType.RequestBody);
            return await client.ExecuteTaskAsync<WebApiResponse>(request);
        }

        public static async Task<IRestResponse<WebApiResponse>> Get(string url)
        {
            var client = new RestClient(Settings.ItmsBaseUrl);
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("UID", "123456");
            request.AddHeader("Token", "abcdef");
            return await client.ExecuteTaskAsync<WebApiResponse>(request);
        }

        public static async Task<IRestResponse<WebApiResponse>> PostAsync(string url, string body = null)
        {
            var client = new RestClient(Settings.ItmsBaseUrl);
            var request = new RestRequest(url, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("UID", "123456");
            request.AddHeader("Token", "abcdef");
            request.AddJsonBody(body);
            return await client.ExecuteTaskAsync<WebApiResponse>(request);
        }

        public static IRestResponse<WebApiResponse> Post(string url, string body = null)
        {
            var client = new RestClient(Settings.ItmsBaseUrl);
            var request = new RestRequest(url, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("UID", "123456");
            request.AddHeader("Token", "abcdef");
            request.AddJsonBody(body);
            return client.Execute<WebApiResponse>(request);
        }
    }
}