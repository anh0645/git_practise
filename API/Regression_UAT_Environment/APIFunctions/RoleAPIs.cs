using Newtonsoft.Json;
using Regression_UAT_Environment.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Regression_UAT_Environment
{
    public static class RoleAPIs
    {
        public static HttpClient client = HttpClientFactory.createDefaultHttpClient();
        private static string requestURL = $"{GlobalVariables.DOMAIN_SERVER}/api/v1/roles";

        public static Tuple<HttpStatusCode, String> Create_Role(Role roleDetails)
        {
            var json = JsonConvert.SerializeObject(roleDetails);
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

        public static Tuple<HttpStatusCode, String> Get_Role_Details(string roleId)
        {
            var builder = new UriBuilder($"{requestURL}/{roleId}");
            builder.Query = $"id={roleId}";
            var url = builder.ToString();
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }

        public static Tuple<HttpStatusCode, String> Get_Role_By_Customer_Id(string customerId)
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
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

        public static Tuple<HttpStatusCode, String> Update_Role_By_Id(string roleId, Role roleDetails)
        {
            var json = JsonConvert.SerializeObject(roleDetails);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri(requestURL + $"/{roleId}"),
                Method = HttpMethod.Patch,
                Content = requestBody
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }

        public static Tuple<HttpStatusCode, String> Delete_Role_By_ID(string roleId)
        {
            var builder = new UriBuilder($"{requestURL}/{roleId}");
            builder.Query = $"id={roleId}";
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri($"{requestURL}/{roleId}"),
                Method = HttpMethod.Delete,
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
