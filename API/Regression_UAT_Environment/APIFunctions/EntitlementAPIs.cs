using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Regression_UAT_Environment.Entities;
using System.Collections.Specialized;

namespace Regression_UAT_Environment
{
    public static class EntitlementAPIs
    {
        // Declare the variables
        public static HttpClient client = HttpClientFactory.createDefaultHttpClient();
        private static string requestURL = $"{GlobalVariables.DOMAIN_SERVER}/api/v1/entitlements";


        public static Tuple<HttpStatusCode, String> Create_Entitlements_For_The_Product_of_Customer(Entitlement EntitlementDetails)
        {

            var json = JsonConvert.SerializeObject(EntitlementDetails);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri(requestURL),
                Method = HttpMethod.Post,
                Content = requestBody
            };
            // Send HTTP request and get all users
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(Entitlement EntitlementDetails)
        {

            var json = JsonConvert.SerializeObject(EntitlementDetails);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri(requestURL),
                Method = HttpMethod.Delete,
                Content = requestBody
            };
            // Send HTTP request and get all users
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }


        public static Tuple<HttpStatusCode, String> Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id(String email, String customerId)
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (email != null)
            {
                queryString.Add("email", email);
            }
            if (customerId != null)
            {
                queryString.Add("customerId", customerId);
            }
            var builder = new UriBuilder(requestURL);
            builder.Query = queryString.ToString();
            var url = builder.ToString();
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }

        public static Tuple<HttpStatusCode, String> Set_DefaultCustomer_for_User(SetDefaultCustomer setDefaultCustomer)
        {
            var json = JsonConvert.SerializeObject(setDefaultCustomer);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri(requestURL),
                Method = HttpMethod.Patch,
                Content = requestBody
            };
            // Send HTTP request and get all users
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }

    }
}
