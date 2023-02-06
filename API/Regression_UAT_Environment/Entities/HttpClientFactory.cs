using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Regression_UAT_Environment.Entities
{
    public static class HttpClientFactory
    {
        public static HttpClient createDefaultHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(GlobalVariables.API_KEY);
            return httpClient;
        }
    }
}
