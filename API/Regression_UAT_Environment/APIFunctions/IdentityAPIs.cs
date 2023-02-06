using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Regression_UAT_Environment.Entities;

namespace Regression_UAT_Environment
{
    public static class IdentityAPIs
    {
        // Declare the variables
        public static HttpClient client = HttpClientFactory.createDefaultHttpClient();
        private static string requestURL = $"{GlobalVariables.DOMAIN_SERVER}/api/v1/identity";


        public static Tuple<HttpStatusCode, String> Create_Missing_B2C_User_For_The_Customer(string customerId)
        {
            var builder = new UriBuilder($"{requestURL}/{customerId}/users-sync");
            builder.Query = $"customerId ={customerId}";
            var url = builder.ToString();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = null
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Create_B2C_User(User UserDetails)
        {
            var json = JsonConvert.SerializeObject(UserDetails);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri($"{requestURL}/users"),
                Method = HttpMethod.Post,
                Content = requestBody
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }

    }
}
