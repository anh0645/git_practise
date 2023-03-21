using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Regression_UAT_Environment.Entities;

namespace Regression_UAT_Environment
{
    public static class ProductAPIs
    {
        // Declare the variables
        public static HttpClient client = HttpClientFactory.createDefaultHttpClient();
        private static string requestURL = $"{GlobalVariables.DOMAIN_SERVER}/api/v1/products";


        public static Tuple<HttpStatusCode, String> Get_All_Available_Products()
        {
            var builder = new UriBuilder(requestURL);
            var url = builder.ToString();
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            } 
        }


        public static Tuple<HttpStatusCode, String> Get_Product_By_Id(string id)
        {
            var builder = new UriBuilder($"{requestURL}/{id}");
            builder.Query = $"id={id}";
            var url = builder.ToString();
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Delete_Product_From_Entitlements_By_Id(string id)
        {
            
            var builder = new UriBuilder($"{requestURL}/{id}");
            builder.Query = $"id={id}";
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri($"{requestURL}/{id}"),
                Method = HttpMethod.Delete
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Update_Product_By_Id(string id, Product ProductDetails)
        {
            var builder = new UriBuilder(requestURL + $"/{id}");
            builder.Query = $"id={id}";
            var json = JsonConvert.SerializeObject(ProductDetails);
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


        public static Tuple<HttpStatusCode, String> Create_Products(ProductList ProductListDetails)
        {
            var json = JsonConvert.SerializeObject(ProductListDetails);
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

        public static Tuple<HttpStatusCode, String> Create_Product(ProductList ProductDetails)
        {
            var json = JsonConvert.SerializeObject(ProductDetails);
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
    }
}
