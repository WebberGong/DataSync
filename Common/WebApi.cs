using System.Threading.Tasks;
using Entity;
using Newtonsoft.Json;
using RestSharp;

namespace Common
{
    public class WebApi
    {
        public static async Task<IRestResponse<WebApiResponse>> PostAsync(string url, object dto)
        {
            var client = new RestClient(Settings.ItmsBaseUrl);
            var request = new RestRequest(url, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("UID", "123456");
            request.AddHeader("Token", "abcdef");
            request.AddParameter("application/json", JsonConvert.SerializeObject(dto), ParameterType.RequestBody);
            return await client.ExecuteTaskAsync<WebApiResponse>(request);
        }

        public static IRestResponse<WebApiResponse> Post(string url, object dto)
        {
            var client = new RestClient(Settings.ItmsBaseUrl);
            var request = new RestRequest(url, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("UID", "123456");
            request.AddHeader("Token", "abcdef");
            request.AddParameter("application/json", JsonConvert.SerializeObject(dto), ParameterType.RequestBody);
            return client.Execute<WebApiResponse>(request);
        }

        public static async Task<bool> SimplePostAsync(string url, object dto)
        {
            var response = await PostAsync(url, dto);
            return Utility.IsResponseSuccess(response);
        }

        public static bool SimplePost(string url, object dto)
        {
            var response = Post(url, dto);
            return Utility.IsResponseSuccess(response);
        }

        public static async Task<IRestResponse<WebApiResponse>> PostAsync(string url, string body)
        {
            var client = new RestClient(Settings.ItmsBaseUrl);
            var request = new RestRequest(url, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("UID", "123456");
            request.AddHeader("Token", "abcdef");
            request.AddJsonBody(body);
            return await client.ExecuteTaskAsync<WebApiResponse>(request);
        }

        public static IRestResponse<WebApiResponse> Post(string url, string body)
        {
            var client = new RestClient(Settings.ItmsBaseUrl);
            var request = new RestRequest(url, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("UID", "123456");
            request.AddHeader("Token", "abcdef");
            request.AddJsonBody(body);
            return client.Execute<WebApiResponse>(request);
        }

        public static async Task<IRestResponse<WebApiResponse>> GetAsync(string url)
        {
            var client = new RestClient(Settings.ItmsBaseUrl);
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("UID", "123456");
            request.AddHeader("Token", "abcdef");
            return await client.ExecuteTaskAsync<WebApiResponse>(request);
        }

        public static IRestResponse<WebApiResponse> Get(string url)
        {
            var client = new RestClient(Settings.ItmsBaseUrl);
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("UID", "123456");
            request.AddHeader("Token", "abcdef");
            return client.Execute<WebApiResponse>(request);
        }
    }
}