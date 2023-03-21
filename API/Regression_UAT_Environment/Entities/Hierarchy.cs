using System.Collections.Generic;


namespace Regression_UAT_Environment
{
    public class Hierarchy
    {
        public string? name { get; set; }
        public string? environmentId { get; set; }
        public string? customerId { get; set; }
    }

    public static class HierarchyFactory
    {
        public static Hierarchy createHierarchy(string name, string environmentId, string customerId)
        {
            if (customerId == null)
            {
                return new Hierarchy
                {
                    name = name,
                    environmentId = environmentId
                };
            }
            else if (name == null)
            {
                return new Hierarchy
                {
                    environmentId = environmentId,
                    customerId = customerId
                };
            }
            else if (name == null && environmentId ==  null)
            {
                return new Hierarchy
                {
                    customerId = customerId
                };
            }
            else
                return new Hierarchy
                {
                    name = name,
                    environmentId = environmentId,
                    customerId = customerId
                };
        }
        public static Hierarchy updateHierarchy(string name)
        {
            return new Hierarchy
            {
                name = name
            };
        }
    }
}
