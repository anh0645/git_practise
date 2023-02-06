using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Regression_UAT_Environment.Entities;
using System.Collections.Specialized;

namespace Regression_UAT_Environment
{
    public static class EnvironmentAPIs
    {
        // Declare the variables
        public static HttpClient client = HttpClientFactory.createDefaultHttpClient();
        private static string requestURL = $"{GlobalVariables.DOMAIN_SERVER}/api/v1/environments";
        
        public static Tuple<HttpStatusCode, String> Get_All_Environments()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri(requestURL),
                Method = HttpMethod.Get
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }

        public static Tuple<HttpStatusCode, String> Get_All_Environments_with_ProductID(string productId)
        {
            var builder = new UriBuilder(requestURL);
            builder.Query = $"productId={productId}";
            var url = builder.ToString();
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Create_New_Product_Environment(Environment EnvironmentDetails)
        {   
            var json = JsonConvert.SerializeObject(EnvironmentDetails);
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


        public static Tuple<HttpStatusCode, String> Delete_Environment_From_Environments_By_ID(String id)
        {
            var builder = new UriBuilder(requestURL + $"/{id}");
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


        public static Tuple<HttpStatusCode, String> Update_Product_Environment_Details(String id, Environment EnvironmentDetails)
        {
            var builder = new UriBuilder(requestURL + $"/{id}");
            builder.Query = $"id={id}";
            var json = JsonConvert.SerializeObject(EnvironmentDetails);
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

        public static Tuple<HttpStatusCode, String> Associate_Customer_With_Product_Environment(String id, Associate AssociateDetails)
        {
            var builder = new UriBuilder(requestURL + $"/{id}");
            builder.Query = $"id={id}";
            var json = JsonConvert.SerializeObject(AssociateDetails);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri(requestURL + $"/{id}"),
                Method = HttpMethod.Post,
                Content = requestBody
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Disassociate_Customer_From_Product_Environment(String productEnvironmentId, String customerId)
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            
            if (productEnvironmentId == null || customerId == null)
            {
                return new Tuple<HttpStatusCode, String>(HttpStatusCode.NotImplemented, null);
            }
            else
            {
                queryString.Add("productEnvironmentId", productEnvironmentId);
                queryString.Add("customerId", customerId);
                var builder = new UriBuilder(requestURL);
                builder.Query = queryString.ToString();
                var request = new HttpRequestMessage
                {
                    RequestUri = new System.Uri(requestURL + $"/{productEnvironmentId}/customers/{customerId}"),
                    Method = HttpMethod.Delete
                };
                using (var response = client.SendAsync(request).Result)
                {
                    return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
                }
            }
        }
    }
}
