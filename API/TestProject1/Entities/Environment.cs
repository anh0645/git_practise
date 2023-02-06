namespace Regression_UAT_Environment
{
    public class Environment
    {
        public string? id { get; set; }
        public string? productId { get; set; }
        public string? name { get; set; }
        public string? url { get; set; }
        public string? code { get; set; }
    }

    public static class EnvironmentFactory
    {
        public static Environment createEnvironment(string id, string productId, string name, string url, string code)
        {
            if(id == null)
            {
                return new Environment
                {
                    productId = productId,
                    name = name,
                    url = url,
                    code = code
                };
            }
            else if (name == null)
            {
                return new Environment
                {
                    id = id,
                    productId = productId,
                    url = url,
                    code = code
                };
            }
            else if(id == null && productId == null)
            {
                return new Environment
                {
                    name = name,
                    url = url,
                    code = code
                };
            }
            else if (id == null && productId == null && name == null)
            {
                return new Environment
                {
                    url = url,
                    code = code
                };
            }
            else
            {
                return new Environment
                {
                    id = id,
                    productId = productId,
                    name = name,
                    url = url,
                    code = code
                };
            }
        }
    }



    public class Associate
    {
        public string? customerId { get; set; }
        public bool? active { get; set; }
    }

    public static class AssociateFactory
    {
        public static Associate createAssociate(string customerId, bool active)
        {
            return new Associate
            {
                customerId = customerId,
                active = active
            };
        }

        public static Associate createAssociate(string customerId)
        {
            return new Associate
            {
                customerId = customerId,
                active = null
            };
        }
    }
}
