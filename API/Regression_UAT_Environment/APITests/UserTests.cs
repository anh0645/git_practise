using Xunit;
using System.Net;
using Xunit.Abstractions;
using System;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Regression_UAT_Environment.Utils;
using System.Collections.Generic;

namespace Regression_UAT_Environment.Tests
{
    public class UserTests : IDisposable
    {
        // Declare the variables
        private readonly ITestOutputHelper logger;
        private string customerId;
        private string productLineId;
        private string productId;
        private string productEnvironmentId;
        private string productIconColor = String.Format("#{0:X6}", new Random().Next(0x1000000));
        private string solutionFamilyId;
        private string solutionFamilyName;
        private string solutionFamilyCode;
        private SolutionFamily solutionFamilyDetails;
        private Entitlement userWithProductDetails;
        private Associate associateCustomerDetails;

        // Initialize the varible using in script
        public UserTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        //Teardown
        public void Dispose()
        {
            // Clean up
            //Delete User
            if (userWithProductDetails != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteUserWithProduct = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(userWithProductDetails);
            }
            //Disassociate Customer And Product Environment
            if (associateCustomerDetails != null)
            {
                Tuple<HttpStatusCode, String> responseDisassociateCustomer = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment(productEnvironmentId, customerId);
            }
            //Cleanup ProductEnv
            if (productEnvironmentId != null)
            {

                Tuple<HttpStatusCode, String> responseDeleteProductEnvironment1 = EnvironmentAPIs.Delete_Environment_From_Environments_By_ID(productEnvironmentId);
            }
            //Cleanup Product
            if (productId != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteProduct = ProductAPIs.Delete_Product_From_Entitlements_By_Id(productId);
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
            logger.WriteLine("Successfully Cleanup the User Tests");
        }

        [Fact]
        public void Test784187_Create_User_with_Valid_Email_and_Displayname()
        {
            string displayName = $"B2CUser_{StringUtils.RandomString(10)}";
            var B2CUserDetails = IdentityFactory.createIdentity($"email_{StringUtils.RandomString(15)}@gmail.com", displayName);
            Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_B2C_User(B2CUserDetails);
            logger.WriteLine("Response Create User with Valid Email and Displayname: " + responseCreateB2CUser);
            Assert.Equal(expected: HttpStatusCode.Created, responseCreateB2CUser.Item1);
        }

        [Fact]
        public void Test776994_Get_User_with_valid_email()
        {
            //Run the Setup function to get customerId and productId1
            Setup_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userEmail = $"email_{StringUtils.RandomString(10)}@gmail.com";
            userWithProductDetails = EntitlementFactory.createEntitlement(productId, customerId, userEmail, userEmail.Replace("@gmail.com", ""), true);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            logger.WriteLine("Response Create User with valid Email: " + responseCreateUserWithProduct);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct.Item1);

            //Get user with valid Email
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(userEmail);
            logger.WriteLine("Response Get User with Valid Email: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);
            Regex _isGuid = new Regex(@"[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?", RegexOptions.Compiled);
            Assert.True(_isGuid.IsMatch(JObject.Parse(responseGetUser.Item2)["id"].ToString()), $"{JObject.Parse(responseGetUser.Item2)["id"].ToString()} is NOT a GUID format.");
        }

        [Fact]
        public void Test776995_Get_User_with_blank_Email()
        {
            //Get user with valid Email
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(null);
            logger.WriteLine("Response Get User with blank Email: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseGetUser.Item1);
        }

        [Fact]
        public void Test776996_Get_User_With_Invalid_Email()
        {
            //Get user with invalid Email
            string userEmail = $"invalid_email_{StringUtils.RandomString(10)}@gmail.com";
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(userEmail);
            logger.WriteLine("Response Get User with Invalid Email: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.NotFound, responseGetUser.Item1);
        }

        [Fact]
        public void Test777005_Update_User_Details_With_Valid_GUID_Entitlement_And_New_Email_Not_In_Entitlement_B2C()
        {
            //Run the Setup function to get customerId and productId1
            Setup_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userEmail = $"email_{StringUtils.RandomString(10)}@gmail.com";
            userWithProductDetails = EntitlementFactory.createEntitlement(productId, customerId, userEmail, userEmail.Replace("@gmail.com", ""), false);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct.Item1);

            //Get userId
            Tuple<HttpStatusCode, String> responseGetUserWithProduct = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id(userEmail, null);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUserWithProduct.Item1);
            string userId = JObject.Parse(responseGetUserWithProduct.Item2)["id"].ToString();

            //Update User Details
            var userDetails = UserFactory.createUser($"newEmail_{StringUtils.RandomString(10)}@gmail.com");
            Tuple<HttpStatusCode, String> responseUpdateUser = UserAPIs.Update_User_Details_By_Id(userId, userDetails);
            logger.WriteLine("Response Update User with Valid ID and Email not in EnvitlementB2C: " + responseCreateUserWithProduct);
            Assert.Equal(expected: HttpStatusCode.OK, responseUpdateUser.Item1);
        }

        [Fact]
        public void Test777008_Update_User_Details_with_Valid_GUID_Entitlement_and_New_Email_Entitlement_B2C()
        {
            //Run the Setup function to get customerId and productId1
            Setup_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            List<string> listUserEmail = new List<string> { $"email_{StringUtils.RandomString(10)}@gmail.com", $"email_{StringUtils.RandomString(10)}@gmail.com" };
            List<string> listDisplayName = new List<string> { $"email_{StringUtils.RandomString(10)}@gmail.com", $"email_{StringUtils.RandomString(10)}@gmail.com" };
            List<bool> listLocalAccount = new List<bool> { false, true };
            userWithProductDetails = EntitlementFactory.createEntitlementWithMultipleUsers(productId, customerId, listUserEmail, listDisplayName, listLocalAccount);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct.Item1);

            //Get user with valid Email
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(listUserEmail[0]);
            string userID = JObject.Parse(responseGetUser.Item2)["id"].ToString();
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);

            //Update User Details
            var userDetails = UserFactory.createUser(listUserEmail[1]);
            Tuple<HttpStatusCode, String> responseUpdateUser = UserAPIs.Update_User_Details_By_Id(userID, userDetails);
            logger.WriteLine("Response Update User with Valid Entitlement ID and Email in Entitlement B2C: " + responseUpdateUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateUser.Item1);
        }

        [Fact]
        public void Test777010_Update_User_Details_with_Valid_GUID_Entitlement_and_New_Email_Entitlement()
        {
            //Run the Setup function to get customerId and productId1
            Setup_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            List<string> listUserEmail = new List<string> { $"email_{StringUtils.RandomString(10)}@gmail.com", $"email_{StringUtils.RandomString(10)}@gmail.com" };
            List<string> listDisplayName = new List<string> { $"email_{StringUtils.RandomString(10)}@gmail.com", $"email_{StringUtils.RandomString(10)}@gmail.com" };
            List<bool> listLocalAccount = new List<bool> { false, false };
            userWithProductDetails = EntitlementFactory.createEntitlementWithMultipleUsers(productId, customerId, listUserEmail, listDisplayName, listLocalAccount);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct.Item1);

            //Get user with valid Email
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(listUserEmail[0]);
            string userID = JObject.Parse(responseGetUser.Item2)["id"].ToString();
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);

            //Update User Details
            var userDetails = UserFactory.createUser(listUserEmail[1]);
            Tuple<HttpStatusCode, String> responseUpdateUser = UserAPIs.Update_User_Details_By_Id(userID, userDetails);
            logger.WriteLine("Response Update User with Valid Entitlement ID and Email in Entitlement: " + responseUpdateUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateUser.Item1);
        }

        [Fact]
        public void Test777012_Update_User_Details_With_Valid_GUID_Entitlement_And_New_Email_B2C()
        {
            //Run the Setup function to get customerId and productId1
            Setup_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userEmail = $"email_{StringUtils.RandomString(10)}@gmail.com";
            userWithProductDetails = EntitlementFactory.createEntitlement(productId, customerId, userEmail, userEmail.Replace("@gmail.com", ""), false);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct.Item1);

            //Create B2C User
            string displayName = StringUtils.RandomString(15);
            var B2CUserDetails = IdentityFactory.createIdentity($"email_{displayName}@gmail.com", displayName);
            Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_B2C_User(B2CUserDetails);
            Assert.Equal(expected: HttpStatusCode.Created, responseCreateB2CUser.Item1);

            //Get user with valid Email
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(userEmail);
            string userID = JObject.Parse(responseGetUser.Item2)["id"].ToString();
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);

            //Update User Details
            var userDetails = UserFactory.createUser($"email_{displayName}@gmail.com");
            Tuple<HttpStatusCode, String> responseUpdateUser = UserAPIs.Update_User_Details_By_Id(userID, userDetails);
            logger.WriteLine("Response Update User with Valid Entitlement ID and new Email B2: " + responseUpdateUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseUpdateUser.Item1);
        }

        [Fact]
        public void Test777014_Update_User_Details_with_Valid_GUID_Entitlement_B2C_and_New_Email_Entitlement_B2C()
        {
            //Run the Setup function to get customerId and productId1
            Setup_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            List<string> listUserEmail = new List<string> { $"email_{StringUtils.RandomString(10)}@gmail.com", $"email_{StringUtils.RandomString(10)}@gmail.com" };
            List<string> listDisplayName = new List<string> { $"email_{StringUtils.RandomString(10)}@gmail.com", $"email_{StringUtils.RandomString(10)}@gmail.com" };
            List<bool> listLocalAccount = new List<bool> { true, true };
            userWithProductDetails = EntitlementFactory.createEntitlementWithMultipleUsers(productId, customerId, listUserEmail, listDisplayName, listLocalAccount);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct.Item1);

            //Get user with valid Email
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(listUserEmail[0]);
            string userID = JObject.Parse(responseGetUser.Item2)["id"].ToString();
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);

            //Update User Details
            var userDetails = UserFactory.createUser(listUserEmail[1]);
            Tuple<HttpStatusCode, String> responseUpdateUser = UserAPIs.Update_User_Details_By_Id(userID, userDetails);
            logger.WriteLine("Response Update User with Valid Entitlement ID and Email in Entitlement: " + responseUpdateUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateUser.Item1);
        }

        [Fact]
        public void Test777020_Update_User_Details_with_Valid_GUID_Entitlement_B2C_and_New_Email_not_In_Entitlement_B2C()
        {
            //Run the Setup function to get customerId and productId1
            Setup_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userEmail = $"email_{StringUtils.RandomString(10)}@gmail.com";
            userWithProductDetails = EntitlementFactory.createEntitlement(productId, customerId, userEmail, userEmail.Replace("@gmail.com", ""), true);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct.Item1);

            //Get user with valid Email
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(userEmail);
            string userID = JObject.Parse(responseGetUser.Item2)["id"].ToString();
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);

            //Update User Details
            var userDetails = UserFactory.createUser($"new_{userEmail}");
            Tuple<HttpStatusCode, String> responseUpdateUser = UserAPIs.Update_User_Details_By_Id(userID, userDetails);
            logger.WriteLine("Response Update User with Valid Entitlement ID and new Email B2C: " + responseUpdateUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseUpdateUser.Item1);
        }

        [Fact]
        public void Test777070_Update_User_Details_With_Valid_GUID_Entitlement_B2C_And_New_Email_Entitlement()
        {
            //Run the Setup function to get customerId and productId1
            Setup_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            List<string> listUserEmail = new List<string> { $"email_{StringUtils.RandomString(10)}@gmail.com", $"email_{StringUtils.RandomString(10)}@gmail.com" };
            List<string> listDisplayName = new List<string> { $"email_{StringUtils.RandomString(10)}@gmail.com", $"email_{StringUtils.RandomString(10)}@gmail.com" };
            List<bool> listLocalAccount = new List<bool> { true, false };
            userWithProductDetails = EntitlementFactory.createEntitlementWithMultipleUsers(productId, customerId, listUserEmail, listDisplayName, listLocalAccount);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct.Item1);

            //Get user with valid Email
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(listUserEmail[0]);
            string userID = JObject.Parse(responseGetUser.Item2)["id"].ToString();
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);

            //Update User Details
            var userDetails = UserFactory.createUser(listUserEmail[1]);
            Tuple<HttpStatusCode, String> responseUpdateUser = UserAPIs.Update_User_Details_By_Id(userID, userDetails);
            logger.WriteLine("Response Update User with Valid Entitlement B2C ID and Email in Entitlement: " + responseUpdateUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateUser.Item1);
        }

        [Fact]
        public void Test778934_Update_User_Details_with_Valid_GUID_Entitlement_B2C_And_New_Email_B2C()
        {
            //Run the Setup function to get customerId and productId1
            Setup_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userEmail = $"email_{StringUtils.RandomString(10)}@gmail.com";
            userWithProductDetails = EntitlementFactory.createEntitlement(productId, customerId, userEmail, userEmail.Replace("@gmail.com", ""), true);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct.Item1);

            //Create B2C User
            string displayName = StringUtils.RandomString(15);
            var B2CUserDetails = IdentityFactory.createIdentity($"email_{displayName}@gmail.com", displayName);
            Tuple<HttpStatusCode, String> responseCreateB2CUser = IdentityAPIs.Create_B2C_User(B2CUserDetails);
            Assert.Equal(expected: HttpStatusCode.Created, responseCreateB2CUser.Item1);

            //Get user with valid Email
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(userEmail);
            string userID = JObject.Parse(responseGetUser.Item2)["id"].ToString();
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);

            //Update User Details
            var userDetails = UserFactory.createUser($"email_{displayName}@gmail.com");
            Tuple<HttpStatusCode, String> responseUpdateUser = UserAPIs.Update_User_Details_By_Id(userID, userDetails);
            logger.WriteLine("Response Update User with Valid Entitlement B2C ID and new Email B2C: " + responseUpdateUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseUpdateUser.Item1);
        }

        [Fact]
        public void Test778935_Update_User_Details_with_Invalid_GUID_Entitlement_and_New_Email_not_In_Entitlement_B2C()
        {
            //Run the Setup function to get customerId and productId1
            Setup_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userEmail = $"email_{StringUtils.RandomString(10)}@gmail.com";
            userWithProductDetails = EntitlementFactory.createEntitlement(productId, customerId, userEmail, userEmail.Replace("@gmail.com", ""), false);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct.Item1);

            //Get user with valid Email
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(userEmail);
            string userID = JObject.Parse(responseGetUser.Item2)["id"].ToString();
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);

            //Update User Details
            var userDetails = UserFactory.createUser($"new_{userEmail}");
            Tuple<HttpStatusCode, String> responseUpdateUser = UserAPIs.Update_User_Details_By_Id($"Invalid_{userID}", userDetails);
            logger.WriteLine("Response Update User with Invalid Entitlement ID and new Email not in Entitlement B2C: " + responseUpdateUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateUser.Item1);
        }

        [Fact]
        public void Test778939_Update_User_Details_with_Blank_GUID_Entitlement_and_New_Email_not_In_Entitlement_B2C()
        {
            //Run the Setup function to get customerId and productId1
            Setup_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userEmail = $"email_{StringUtils.RandomString(10)}@gmail.com";
            userWithProductDetails = EntitlementFactory.createEntitlement(productId, customerId, userEmail, userEmail.Replace("@gmail.com", ""), false);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct.Item1);

            //Get user with valid Email
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email(userEmail);
            string userID = JObject.Parse(responseGetUser.Item2)["id"].ToString();
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);

            //Update User Details
            var userDetails = UserFactory.createUser($"new_{userEmail}");
            Tuple<HttpStatusCode, String> responseUpdateUser = UserAPIs.Update_User_Details_By_Id(null, userDetails);
            logger.WriteLine("Response Update User with blank Entitlement ID and new Email not in Entitlement B2C: " + responseUpdateUser);
            Assert.Equal(expected: HttpStatusCode.NotImplemented, responseUpdateUser.Item1);
        }



        //Setup 2 Product Environment function
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
            string productLineName = $"ProductLine_{StringUtils.RandomString(10)}";
            var productLineDetails = ProductLineFactory.createProductLine(productLineId, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProductLine = ProductLineAPIs.Create_New_ProductLine(productLineDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductLine.Item1);

            // Create new Solution Family
            solutionFamilyId = $"{Guid.NewGuid()}";
            solutionFamilyName = $"Solution_Family_anh_{StringUtils.RandomString(10)}";
            solutionFamilyCode = StringUtils.RandomString(5);
            solutionFamilyDetails = SolutionFamilyFactory.createSolutionFamily(solutionFamilyId, solutionFamilyName, solutionFamilyCode, productLineId);
            Tuple<HttpStatusCode, String> responseCreateSolutionFamily = SolutionFamilyAPIs.Create_Solution_Family(solutionFamilyDetails);
            logger.WriteLine("New SolutionFamily:" + responseCreateSolutionFamily.Item2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateSolutionFamily.Item1);

            // Create new product(1)
            productId = $"{Guid.NewGuid()}";
            string productName1 = $"Product_{StringUtils.RandomString(10)}";
            string productCode1 = StringUtils.RandomString(5);
            string type = "API";
            var productDetails1 = ProductFactory.createProductList(productId, productName1, productCode1, productIconColor, productLineName, solutionFamilyId, type);
            Tuple<HttpStatusCode, String> responseCreateProduct1 = ProductAPIs.Create_Products(productDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProduct1.Item1);

            // Create new product environment(1)
            productEnvironmentId = $"{Guid.NewGuid()}";
            string productEnvironmentName1 = $"Product_Environment_{StringUtils.RandomString(10)}";
            string urlProductEnvironment1 = $"https://testAPI.com/{productEnvironmentId}";
            string productEnvironmentCode1 = StringUtils.RandomString(5);
            var productEnvironmentDetails1 = EnvironmentFactory.createEnvironment(productEnvironmentId, productId, productEnvironmentName1, urlProductEnvironment1, productEnvironmentCode1);
            Tuple<HttpStatusCode, String> responseCreateProductEnvironment1 = EnvironmentAPIs.Create_New_Product_Environment(productEnvironmentDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductEnvironment1.Item1);

            //Associate Customer And Product Environment(1)
            associateCustomerDetails = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer1 = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId, associateCustomerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer1.Item1);
        }
    }
}