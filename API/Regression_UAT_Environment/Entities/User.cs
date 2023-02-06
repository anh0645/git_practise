namespace Regression_UAT_Environment
{
    public class User
    {
        public string? email { get; set; }
        public string? displayName { get; set; }
        public bool? localAccount { get; set; }
    }


    public static class UserFactory
    {
        public static User createUser(string email)
        {
            return new User
            {
                email = email
            };
           
        }
    }

}
