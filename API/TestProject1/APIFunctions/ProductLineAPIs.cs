using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Regression_UAT_Environment.Entities;

namespace Regression_UAT_Environment
{
    public static class ProductLineAPIs
    {
        // Declare the variables
        public static HttpClient client = HttpClientFactory.createDefaultHttpClient();
        private static string requestURL = $"{GlobalVariables.DOMAIN_SERVER}/api/v1/product-lines";


        public static Tuple<HttpStatusCode, String> Get_All_Available_ProductLines()
        {
            var builder = new UriBuilder(requestURL);
            var url = builder.ToString();
            // Send HTTP request and get all users
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            } 
        }


        public static Tuple<HttpStatusCode, String> Create_New_ProductLine(ProductLine ProductLineDetails)
        {
            var json = JsonConvert.SerializeObject(ProductLineDetails);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri(requestURL),
                Method = HttpMethod.Post,
                Content = requestBody
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Get_ProductLine_By_Id(string id)
        {
            var builder = new UriBuilder($"{requestURL}/{id}");
            builder.Query = $"id={id}";
            var url = builder.ToString();
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Delete_ProductLine_By_Id(string id)
        {
            var builder = new UriBuilder($"{requestURL}/{id}");
            builder.Query = $"id={id}";
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri(requestURL + $"/{id}"),
                Method = HttpMethod.Delete
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Update_ProductLines_By_Id(string id, ProductLine ProductLineDetails)
        {
            var builder = new UriBuilder(requestURL + $"/{id}");
            builder.Query = $"id={id}";
            var json = JsonConvert.SerializeObject(ProductLineDetails);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri(requestURL + $"/{id}"),
                Method = HttpMethod.Patch,
                Content = requestBody
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
