using System.Collections.Generic;

namespace Regression_UAT_Environment
{
    public class Entitlement
    {
        public string? productId { get; set; }
        public string? customerId { get; set; }
        public List<User>? users { get; set; }
    }

    public class SetDefaultCustomer
    {
        public string? email { get; set; }
        public string? customerId { get; set; }
    }

    public static class EntitlementFactory
    {
        public static Entitlement createEntitlement(string productId, string customerId, string email, string displayName, bool localAccount)
        {
            return new Entitlement
            {
                productId = productId,
                customerId = customerId,
                users = new List<User>()
                    {
                     new User()
                        {
                        email = email,
                        displayName = displayName,
                        localAccount = localAccount
                        }
                    }
            };
        }

        public static Entitlement createEntitlement(string productId, string customerId, string email, string displayName)
        {
            return new Entitlement
            {
                productId = productId,
                customerId = customerId,
                users = new List<User>()
                    {
                     new User()
                        {
                        email = email,
                        displayName = displayName
                        }
                    }
            };
        }

        public static Entitlement createEntitlementWithMultipleUsers(string productId, string customerId, List<string> listEmail, List<string> listDisplayName, List<bool> listLocalAccount)
        {
            List<User> users = new List<User>();
            for (int i = 0; i < listEmail.Count; i++)
            {
                var user = new User()
                {
                    email = listEmail[i],
                    displayName = listDisplayName[i],
                    localAccount = listLocalAccount[i]
                };
                users.Add(user);
            }
            return new Entitlement
            {
                productId = productId,
                customerId = customerId,
                users = users
            };
        }

        public static SetDefaultCustomer setDefaultCustomer(string email, string customerId)
        {
            return new SetDefaultCustomer
            {
                email = email,
                customerId = customerId
            };
        }
    }
}
