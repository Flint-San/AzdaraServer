using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

//nice example for simple unit test https://www.youtube.com/watch?v=XG4LgkTbM_Q
//where _instance MUST be saved to memory for one-time activation an edm library(dll) 
namespace AzdaraServer.Chinook.Tests.Helpers
{
    public class RouteHelper
    {
        private static RouteHelper _instance;
        //private HttpClient _client;
        public HttpClient _client { get; private set; }

        public HttpConfiguration Config { get; private set; }
        
        public static RouteHelper Create()
        {
            if (_instance == null)
            {
                _instance = new RouteHelper();

                _instance.Config = new HttpConfiguration();
                WebApiConfig.Register(_instance.Config);
                var server = new HttpServer(_instance.Config);
                _instance._client = new HttpClient(server);
                _instance._client.BaseAddress = new System.Uri("http://localhost:57394/odata");
                //_instance._client.DefaultRequestHeaders
                //    .Accept
                //    .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            }
            //_instance._client.DefaultRequestHeaders.Clear();
            return _instance;
        }
        
        //i don't need it
        //public HttpResponseMessage Get(string url)
        //{
        //    var responseMessage = _client.GetAsync(url).Result;
        //    return responseMessage;
        //}
    }
}
