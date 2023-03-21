using System.Collections.Generic;


namespace Regression_UAT_Environment
{   
    public class Role
    {
        public RoleDetails role { get; set; }
    }

    public class RoleDetails
    {
        public string? customerId { get; set; }
        public string? productId { get; set; }
        public string? name { get; set; }
        public string? sfRoleId { get; set; }
        public string? hierarchyId { get; set; }
    }

    public static class RoleFactory
    {
        public static Role createRole(string customerId, string productId, string name, string sfRoleId, string hierarchyId)
        {
            if (hierarchyId == null)
            {
                return new Role
                {
                    role = new RoleDetails()
                    {
                        customerId = customerId,
                        productId = productId,
                        name = name,
                        sfRoleId = sfRoleId
                    }
                };
            }
            else if (sfRoleId == null)
                return new Role
                {
                    role = new RoleDetails()
                    {
                        customerId = customerId,
                        productId = productId,
                        name = name,
                        hierarchyId = hierarchyId                      
                    }
                };
            else
                return new Role
                {
                    role = new RoleDetails()
                    {
                        customerId = customerId,
                        productId = productId,
                        name = name,
                        sfRoleId = sfRoleId,
                        hierarchyId = hierarchyId
                    }
                };
        }

        public static Role updateRole(string name, string sfRoleId)
        {
            if (name == null)
            {
                return new Role
                {
                    role = new RoleDetails()
                    {
                        sfRoleId = sfRoleId
                    }
                };
            }   
            else if (sfRoleId == null)
            {
                return new Role
                {
                    role = new RoleDetails()
                    {
                        name = name
                    }
                };
            }
            else
            {
                return new Role
                {
                    role = new RoleDetails()
                    {
                        name = name,
                        sfRoleId = sfRoleId
                    }
                };
            }
        }
    }
}
