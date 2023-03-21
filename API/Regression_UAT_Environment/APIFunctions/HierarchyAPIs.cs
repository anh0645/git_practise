using Newtonsoft.Json;
using Regression_UAT_Environment.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Regression_UAT_Environment
{ 
    public static class HierarchyAPIs
    {
        public static HttpClient client = HttpClientFactory.createDefaultHttpClient();
        private static string requestURL = $"{GlobalVariables.DOMAIN_SERVER}/api/v1/hierarchy";
        public static Tuple<HttpStatusCode, String> Create_Hierarchy(Hierarchy HierarchyDetails)
        {
            var json = JsonConvert.SerializeObject(HierarchyDetails);
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

        public static Tuple<HttpStatusCode, String> Get_Hierarchy_Details(string hierarchyId)
        {
            var builder = new UriBuilder($"{requestURL}/{hierarchyId}");
            builder.Query = $"id={hierarchyId}";
            var url = builder.ToString();
            using (var response = client.GetAsync(url).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }

        public static Tuple<HttpStatusCode, String> Delete_Hierarchy_Details(string hierarchyId)
        {
            var builder = new UriBuilder($"{requestURL}/{hierarchyId}");
            builder.Query = $"id={hierarchyId}";
            var request = new HttpRequestMessage
            {
                RequestUri = new System.Uri($"{requestURL}/{hierarchyId}"),
                Method = HttpMethod.Delete,
            };
            using (var response = client.SendAsync(request).Result)
            {
                return new Tuple<HttpStatusCode, String>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
            }
        }

        public static Tuple<HttpStatusCode, String> Update_Hierarchy_By_Id(string id, Hierarchy HierarchyDetails)
        {
            var json = JsonConvert.SerializeObject(HierarchyDetails);
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
