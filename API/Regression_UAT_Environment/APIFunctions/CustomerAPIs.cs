using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Regression_UAT_Environment.Entities;
using System.Collections.Generic;

namespace Regression_UAT_Environment
{
    public static class CustomerAPIs
    {
        // Declare the variables
        public static HttpClient client = HttpClientFactory.createDefaultHttpClient();
        private static string requestURL = $"{GlobalVariables.DOMAIN_SERVER}/api/v1/customers";

        public static Tuple<HttpStatusCode, String> Get_All_Customers_Details()
        {
            var builder = new UriBuilder(requestURL);
            var url = builder.ToString();
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            } 
        }


        public static Tuple<HttpStatusCode, String> Get_Customers_Details(string customerId)
        {
            var builder = new UriBuilder($"{requestURL}/{customerId}");
            builder.Query = $"id={customerId}";
            var url = builder.ToString();
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Delete_Customers_Details(string customerId)
        {
            var builder = new UriBuilder($"{requestURL}/{customerId}");
            builder.Query = $"id={customerId}";
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri($"{requestURL}/{customerId}"),
                Method = HttpMethod.Delete,
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Update_Customers_Details(string customerId, Customer CustomerDetails)
        {
            var builder = new UriBuilder(requestURL + $"/{customerId}");
            builder.Query = $"id={customerId}";
            var json = JsonConvert.SerializeObject(CustomerDetails);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri(requestURL + $"/{customerId}"),
                Method = HttpMethod.Patch,
                Content = requestBody
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Create_Customers_Details(Customer CustomerDetails)
        {
            var json = JsonConvert.SerializeObject(CustomerDetails);
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
