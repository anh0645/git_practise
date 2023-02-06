using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Regression_UAT_Environment.Entities;
using System.Collections.Specialized;

namespace Regression_UAT_Environment
{
    public static class UserAPIs
    {
        // Declare the variables
        public static HttpClient client = HttpClientFactory.createDefaultHttpClient();
        private static string requestURL = $"{GlobalVariables.DOMAIN_SERVER}/api/v1/users";
        

        public static Tuple<HttpStatusCode, String> Get_User_By_Email(String email)
        {
            var builder = new UriBuilder(requestURL + $"/{email}");
            builder.Query = $"email={email}";
            var url = builder.ToString();
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            } 
        }

        public static Tuple<HttpStatusCode, String> Get_Users_By_Filtered(string customerId, string productId, string email, int page, int pageSize)
        {
            // Convert page and pageSize to String.
            string pageConverted;
            if (page > 0) { pageConverted = page.ToString(); }
            else { pageConverted = ""; }

            string pageSizeConverted;
            if (pageSize > 0) { pageSizeConverted = page.ToString(); }
            else { pageSizeConverted = ""; }

            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            string[] userFilterField = { "customerId", "productId", "email", "page", "pageSize" };
            string[] userFiltervalue = { customerId, productId, email, pageConverted, pageSizeConverted };
            for (int i=0; i<userFilterField.Length; i++)
            {
                if (userFiltervalue[i] != "")
                {
                    queryString.Add(userFilterField[i], userFiltervalue[i]);
                }
            }    
            
            var builder = new UriBuilder(requestURL);
            builder.Query = queryString.ToString();
            var url = builder.ToString();
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Update_User_Details_By_Id(string id, User Object)
        {
            if (id == null)
            {
                return new Tuple<HttpStatusCode, String>(HttpStatusCode.NotImplemented, null);
            }
            else
            {
                var builder = new UriBuilder($"{requestURL}/{id}");
                builder.Query = $"id={id}";
                var json = JsonConvert.SerializeObject(Object);
                var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage
                {
                    RequestUri = new System.Uri($"{requestURL}/{id}"),
                    Method = HttpMethod.Patch,
                    Content = requestBody
                };
                using (var response = client.SendAsync(request).Result)
                {
                    return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
                }
            }
        }


        public static Tuple<HttpStatusCode, String> Update_User_Details_By_Email(string email, User UserDetails)
        {
            var builder = new UriBuilder(requestURL);
            builder.Query = $"email={email}";
            var json = JsonConvert.SerializeObject(UserDetails);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri(requestURL),
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
