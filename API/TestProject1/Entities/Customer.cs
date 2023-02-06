using System.Collections.Generic;

namespace Regression_UAT_Environment
{
    public class Customer
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public bool? active { get; set; }
        public List<ProductEnviroments> productEnvironments { get; set; }
    }

    public class ProductEnviroments
    {
        public string? id { get; set; }
        public string? productId { get; set; }
        public string? name { get; set; }
        public string? url { get; set; }
        public string? code { get; set; }
        public string? vanityUrl { get; set; }
    }


    public static class CustomerFactory
    {
        public static Customer createCustomer(string id, string name, bool active)
        {
            if (id == null)
            {
                return new Customer
                {
                    name = name,
                    active = active
                };
            }
            else
            {
                return new Customer
                {
                    id = id,
                    name = name,
                    active = active
                };
            }

        }
        public static Customer createCustomer(string id, string name)
        {
            return new Customer
            {
                id = id,
                name = name
            };
        }

        public static Customer createCustomer(string id, string name, bool active, string productEnvironmentId, string productId, string productEnvironmentName, string productEnvironmentUrl, string productEnvironmentCode)
        {
            if (id == null)
            {
                return new Customer
                {
                    name = name,
                    active = active,
                    productEnvironments = new List<ProductEnviroments>()
                    {
                        new ProductEnviroments()
                        {
                            id = productEnvironmentId,
                            productId = productId,
                            name = productEnvironmentName,
                            url = productEnvironmentUrl,
                            code = productEnvironmentCode,
                            vanityUrl = productEnvironmentUrl
                        }
                    }
                };
            }
            else
            {
                return new Customer
                {
                    id = id,
                    name = name,
                    active = active,
                    productEnvironments = new List<ProductEnviroments>()
                    {
                        new ProductEnviroments()
                        {
                            id = productEnvironmentId,
                            productId = productId,
                            name = productEnvironmentName,
                            url = productEnvironmentUrl,
                            code = productEnvironmentCode,
                            vanityUrl = productEnvironmentUrl
                        }
                    }
                };
            }
        }
    }
}
