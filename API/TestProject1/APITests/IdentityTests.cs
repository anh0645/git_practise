using Xunit;
using System.Net;
using Xunit.Abstractions;
using System;
using Regression_UAT_Environment.Utils;
using Newtonsoft.Json.Linq;

namespace Regression_UAT_Environment.Tests
{
    public class IdentityTests : IDisposable
    {
        // Declare the variables
        private readonly ITestOutputHelper logger;
        private string customerId;
        private string productLineId;
        private string productLineName;
        private string productId;
        private string productEnvironmentId;
        private string productIconColor = String.Format("#{0:X6}", new Random().Next(0x1000000));
        private Entitlement userWithProductDetails;
        private Associate associateCustomerDetails;

        // Initialize the varible using in script
        public IdentityTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        //Teardown
        public void Dispose()
        {
            //Delete User with Product(1)
            if (userWithProductDetails != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteUserWithProduct1 = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(userWithProductDetails);
            }
            if (associateCustomerDetails != null)
            {
                // Disassociate Customer And Product Environment
                Tuple<HttpStatusCode, String> responseDisassociateCustomer1 = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment(productEnvironmentId, customerId);
            }
            //Cleanup ProductEnv(1)
            if (productEnvironmentId != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteProductEnvironment1 = EnvironmentAPIs.Delete_Environment_From_Environments_By_ID(productEnvironmentId);
            }
            //Cleanup Product(1)
            if (productId != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteProduct1 = ProductAPIs.Delete_Product_From_Entitlements_By_Id(productId);
            }
            //Cleanup ProductLine
            if (productLineId != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteProductLine = ProductLineAPIs.Delete_ProductLine_By_Id(productLineId);
            }
            //Cleanup Customer
            if (customerId != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteCustomer = CustomerAPIs.Delete_Customers_Details(customerId);
            }
            logger.WriteLine("Successfully Cleanup the Identity Tests");
        }

        [Fact]
        public void Test784189_Create_User_with_Valid_Email_and_Blank_Displayname()
        {
            var B2CUserDetails = IdentityFactory.createIdentity($"email{StringUtils.RandomString(15)}@gmail.com", null);
            Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_B2C_User(B2CUserDetails);
            logger.WriteLine("Response Create User with Valid Email and Blank Displayname: " + responseCreateB2CUser);
            Assert.Equal(expected: HttpStatusCode.Created, responseCreateB2CUser.Item1);
        }

        [Fact]
        public void Test784191_Create_User_with_Invalid_Email_and_Valid_Displayname()
        {
            string displayName = $"B2CUser_{StringUtils.RandomString(10)}";
            var B2CUserDetails = IdentityFactory.createIdentity($"email{StringUtils.RandomString(15)}.gmail.com", displayName);
            Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_B2C_User(B2CUserDetails);
            logger.WriteLine("Response Create User with Invalid Email and Valid Displayname: " + responseCreateB2CUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateB2CUser.Item1);
        }

        [Fact]
        public void Test784197_Create_User_With_Blank_Email_And_Blank_Displayname()
        {
            var B2CUserDetails = IdentityFactory.createIdentity(null, null);
            Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_B2C_User(B2CUserDetails);
            logger.WriteLine("Response Create User with Invalid Email and Valid Displayname: " + responseCreateB2CUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateB2CUser.Item1);
        }

        [Fact]
        public void Test784198_Create_User_with_Email_in_Entitlements_DB_and_Valid_Displayname()
        {
            //Create entitlement for the given set of users to the given product of a customer
            Setup_ProductEnvironment_To_Create_User_with_Product();
            string userName = StringUtils.RandomString(10);
            string email = $"email{userName}@gmail.com";
            var userWithProductDetails = EntitlementFactory.createEntitlement(productId, customerId, email, userName, false);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            logger.WriteLine("Response Create Entitlement: " + responseCreateuserWithProduct);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);
            //Create User with Email in Entitlements Database and Valid Displayname
            string displayName = $"B2CUser_{StringUtils.RandomString(10)}";
            var B2CUserDetails = IdentityFactory.createIdentity(email, displayName);
            Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_B2C_User(B2CUserDetails);
            logger.WriteLine("Response Create User with Email in Entitlement Database: " + responseCreateB2CUser);
            Assert.Equal(expected: HttpStatusCode.Created, responseCreateB2CUser.Item1);
        }

        [Fact]
        public void Test784202_Create_User_With_Existing_Email_In_B2C_And_Valid_Displayname()
        {
            //Create entitlement for the given set of users to the given product of a customer
            Setup_ProductEnvironment_To_Create_User_with_Product();
            string userName = StringUtils.RandomString(10);
            string email = $"email{userName}@gmail.com";
            var userWithProductDetails = EntitlementFactory.createEntitlement(productId, customerId, email, userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            logger.WriteLine("Response Create Entitlement: " + responseCreateuserWithProduct);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);
            //Create User with Email in Entitlements Database and Valid Displayname
            string displayName = $"B2CUser_{StringUtils.RandomString(10)}";
            var B2CUserDetails = IdentityFactory.createIdentity(email, displayName);
            Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_B2C_User(B2CUserDetails);
            logger.WriteLine("Response Create User with existing Email in B2C: " + responseCreateB2CUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateB2CUser.Item1);
        }

        [Fact]
        public void Test784314_Create_User_with_Displayname_match_with_Another_User()
        {
            //Create two B2C users with the same displayName
            string displayName = $"BTCUser{StringUtils.RandomString(10)}";
            for(int i=0; i < 2; i++)
            {
                var B2CUserDetails = IdentityFactory.createIdentity($"email{StringUtils.RandomString(15)}@gmail.com", displayName);
                Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_B2C_User(B2CUserDetails);
                logger.WriteLine("Response Create two Users with The Same displayName : " + responseCreateB2CUser);
                Assert.Equal(expected: HttpStatusCode.Created, responseCreateB2CUser.Item1);
            }
        }

        [Fact]
        public void Test788476_Create_Missing_B2C_User_With_Valid_Customer_GUID_Customer_Not_Have_User()
        {
            //Create new user
            String name = "name_test_" + StringUtils.RandomString(10);
            var customer = CustomerFactory.createCustomer(null, name, true);
            Tuple<HttpStatusCode, String> responseCreate = CustomerAPIs.Create_Customers_Details(customer);
            logger.WriteLine("Response Create Customer:" + responseCreate.Item2);
            string customerID = JObject.Parse(responseCreate.Item2)["id"].ToString();
            logger.WriteLine("ID Created: " + customerID);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreate.Item1);

            //Create missing B2C user with valid customer GUID (customer not have user)
            Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_Missing_B2C_User_For_The_Customer(customerID);
            logger.WriteLine("Response Create Missing B2C User with Valid Customer GUID (Customer not have User): " + responseCreateB2CUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateB2CUser.Item1);
        }

        [Fact]
        public void Test787959_Create_Missing_B2C_User_With_Valid_Customer_GUID_Exist_Missing_B2C_User()
        {
            // Create new customer to get customerID
            customerId = $"{Guid.NewGuid()}";
            string customerName = $"Customer_{StringUtils.RandomString(10)}";
            var customerDetails = CustomerFactory.createCustomer(customerId, customerName, true);
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            //Create missing B2C user with valid customer GUID (customer not have user)
            Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_Missing_B2C_User_For_The_Customer(customerId);
            logger.WriteLine("Response Create Missing B2C User with Valid Customer GUID (Customer not have User): " + responseCreateB2CUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateB2CUser.Item1);
        }

        [Fact]
        public void Test787961_Create_Missing_B2C_User_With_Invalid_Customer_GUID()
        {
            //Create missing B2C user with invalid customer GUID
            Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_Missing_B2C_User_For_The_Customer($"Invalid_{Guid.NewGuid()}");
            logger.WriteLine("Response Create Missing B2C User with Invalid Customer GUID: " + responseCreateB2CUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateB2CUser.Item1);
        }


        //Setup Product Environment function
        public void Setup_ProductEnvironment_To_Create_User_with_Product()
        {
            // Create new customer to get customerID
            customerId = $"{Guid.NewGuid()}";
            string customerName = $"Customer_{StringUtils.RandomString(10)}";
            var customerDetails = CustomerFactory.createCustomer(customerId, customerName, true);
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            //Create new productLine
            productLineId = $"{Guid.NewGuid()}";
            productLineName = $"ProductLine_{StringUtils.RandomString(10)}";
            var productLineDetails = ProductLineFactory.createProductLine(productLineId, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProductLine = ProductLineAPIs.Create_New_ProductLine(productLineDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductLine.Item1);

            // Create new product
            productId = $"{Guid.NewGuid()}";
            string productName1 = $"Product_{StringUtils.RandomString(10)}";
            string productCode1 = StringUtils.RandomString(5);
            var productDetails1 = ProductFactory.createProductList(productId, productName1, productCode1, productIconColor, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProduct1 = ProductAPIs.Create_Products(productDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProduct1.Item1);

            // Create new product environment
            productEnvironmentId = $"{Guid.NewGuid()}";
            string productEnvironmentName1 = $"Product_Environment_{StringUtils.RandomString(10)}";
            string urlProductEnvironment1 = $"https://testAPI.com/{productEnvironmentId}";
            string productEnvironmentCode1 = StringUtils.RandomString(5);
            var productEnvironmentDetails1 = EnvironmentFactory.createEnvironment(productEnvironmentId, productId, productEnvironmentName1, urlProductEnvironment1, productEnvironmentCode1);
            Tuple<HttpStatusCode, String> responseCreateProductEnvironment1 = EnvironmentAPIs.Create_New_Product_Environment(productEnvironmentDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductEnvironment1.Item1);

            //Associate Customer And Product Environment
            associateCustomerDetails = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer1 = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId, associateCustomerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer1.Item1);
        }
    }
}