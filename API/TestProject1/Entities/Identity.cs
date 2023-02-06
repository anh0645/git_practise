namespace Regression_UAT_Environment
{
    public static class IdentityFactory
    {
        public static User createIdentity(string email, string displayName)
        {
            if (displayName == null)
            {
                return new User
                {
                    email = email
                };
            }
            else
            {
                return new User
                {
                    email = email,
                    displayName = displayName,
                    firstName = displayName.Substring(0, displayName.Length/2),
                    lastName = displayName.Substring(displayName.Length / 2)
                };
            }
        }
    }

}
