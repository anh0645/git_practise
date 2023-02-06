using Xunit;
using System.Net;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;
using Regression_UAT_Environment.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Regression_UAT_Environment.Tests
{
    public class EntitlementTests : IDisposable
    {
        // Declare the variables
        private readonly ITestOutputHelper logger;
        private string customerId;
        private string productLineId;
        private string productLineName;
        private string productId1;
        private string productId2;
        private string productEnvironmentId1;
        private string productEnvironmentId2;
        private string productIconColor = String.Format("#{0:X6}", new Random().Next(0x1000000));
        private Entitlement userWithProductDetails1;
        private Entitlement userWithProductDetails2;
        private Associate associateCustomerDetails;



        public bool localAccount { get; private set; }
        // Initialize the varible using in script
        public EntitlementTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        //Teardown
        public void Dispose()
        {
            //Cleanup
            //Delete User with Product(1)
            if (userWithProductDetails1 != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteUserWithProduct1 = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(userWithProductDetails1);
            }
            //Delete User with Product(2)
            if (userWithProductDetails2 != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteUserWithProduct2 = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(userWithProductDetails2); 
            } 
            if (associateCustomerDetails != null)
            {
                // Disassociate Customer And Product Environment(1)
                Tuple<HttpStatusCode, String> responseDisassociateCustomer1 = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment(productEnvironmentId1, customerId);
                //Disassociate Customer And Product Environment(2)
                Tuple<HttpStatusCode, String> responseDisassociateCustomer2 = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment(productEnvironmentId2, customerId);
            }
            //Cleanup ProductEnv(1)
            if (productEnvironmentId1 != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteProductEnvironment1 = EnvironmentAPIs.Delete_Environment_From_Environments_By_ID(productEnvironmentId1);
            }
            //Cleanup ProductEnv(2)
            if (productEnvironmentId2 != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteProductEnvironment2 = EnvironmentAPIs.Delete_Environment_From_Environments_By_ID(productEnvironmentId2);
            }
            //Cleanup Product(1)
            if (productId1 != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteProduct1 = ProductAPIs.Delete_Product_From_Entitlements_By_Id(productId1);
            }
            //Cleanup Product(2)
            if (productId2 != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteProduct2 = ProductAPIs.Delete_Product_From_Entitlements_By_Id(productId2);
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
            logger.WriteLine("Successfully Cleanup the Entitlement Tests");
        }

        [Fact]
        public void Test774665_Create_User_With_Valid_CustomerId_And_ProductId_LocalAccountFalse()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product(1)
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, false);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct1 = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            logger.WriteLine("Response Create User with Valid Information and localAccount is false: " + responseCreateuserWithProduct1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct1.Item1);
        }

        [Fact]
        public void Test774683_Create_User_with_Valid_CustomerId_and_ProductId_LocalAccountTrue()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product(1)
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct1 = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            logger.WriteLine("Response Create User with Valid Information and localAccount is true: " + responseCreateuserWithProduct1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct1.Item1);
        }

        [Fact]
        public void Test775651_Create_User_When_Entitlement_User_Is_Already_Exists_LocalAccount_Is_True()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();

            //Create User 1 with product(1)
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, false);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct1 = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            logger.WriteLine("Response Create User 1 with Valid Information and localAccount is false: " + responseCreateuserWithProduct1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct1.Item1);

            //Create User 2 with product(1), same information, localAccount is true
            userWithProductDetails1.users[0].localAccount = true;
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct2 = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            logger.WriteLine("Response Create User 2 with the same Information and localAccount is true: " + responseCreateuserWithProduct2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct2.Item1);
        }

        [Fact]
        public void Test775654_Create_User_with_Valid_ProductId_and_Invalid_CustomerId_LocalAccountTrue()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product(1)
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, $"Invalid_{customerId}", $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct1 = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            logger.WriteLine("Response Create User with Valid Information and localAccount is true: " + responseCreateuserWithProduct1);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateuserWithProduct1.Item1);
        }

        [Fact]
        public void Test775657_Create_User_With_Invalid_ProductId_And_Valid_CustomerId_LocalAccountTrue()
        {
            // Create new customer to get customerID
            customerId = $"{Guid.NewGuid()}";
            string customerName = $"Customer_{StringUtils.RandomString(10)}";
            var customerDetails = CustomerFactory.createCustomer(customerId, customerName, true);
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            //Create User with product(1)
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement($"{Guid.NewGuid()}", customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct1 = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            logger.WriteLine("Response Create User with Invalid ProductId: " + responseCreateuserWithProduct1);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateuserWithProduct1.Item1);
        }

        [Fact]
        public void Test776075_Delete_Entitlement_for_one_User_if_They_Can_Access_to_Multi_Product()
        {
            //Run the Setup function to get customerId, productId1 and productId2
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();
            Setup_2nd_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product(1)
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct1 = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct1.Item1);

            //Create User with product(2)
            userWithProductDetails2 = EntitlementFactory.createEntitlement(productId2, customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateUserWithProduct2 = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateUserWithProduct2.Item1);

            //Delete User with Product(1)
            Tuple<HttpStatusCode, String> responseDeleteUserWithProduct1 = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseDeleteUserWithProduct1.Item1);

            //Get available Solution Families for user by user email to verify Product is exist or not
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id($"email_{userName}@gmail.com", null);
            logger.WriteLine("Response Get Available Solution Families for User by User Email: " + responseGetUser);
            JArray products = (JArray)JObject.Parse(responseGetUser.Item2)["products"];
            logger.WriteLine("Available Products: " + products);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);
            Assert.Equal(expected: 1, products.Count);
            Assert.Equal(expected: productId2, products[0]["productId"]);
        }

        [Fact]
        public void Test776076_Delete_Entitlement_For_Many_User_If_They_Can_Access_To_Multi_Product()
        {
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();
            Setup_2nd_ProductEnvironment_To_Create_User_with_Product();

            //create list information for customers need to create
            List<string> listEmail = new List<string>();
            List<string> listDisplayName = new List<string>();
            List<bool> listLocalAccount = new List<bool>();
            //add information of customers to list
            for (int i = 0; i < 2; i++)
            {
                listEmail.Add($"Customer_{StringUtils.RandomString(10)}@gmail.com");
                listDisplayName.Add($"Customer_{StringUtils.RandomString(10)}");
                listLocalAccount.Add(new Random().Next(10) < 5);
            }

            //create customer details list with productId1
            var customerDetailsList = EntitlementFactory.createEntitlementWithMultipleUsers(productId1, customerId, listEmail, listDisplayName, listLocalAccount);
            Tuple<HttpStatusCode, String> responseCreateCustomerList = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(customerDetailsList);
            logger.WriteLine("Response Create Customer Details List(1): " + responseCreateCustomerList);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomerList.Item1);
            userWithProductDetails1 = customerDetailsList;

            //create customer details list with productId1
            var customerDetailsList2 = customerDetailsList;
            customerDetailsList2.productId = productId2;
            Tuple<HttpStatusCode, String> responseCreateCustomerList2 = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(customerDetailsList2);
            logger.WriteLine("Response Create Customer Details List(2) same users and another productId: " + responseCreateCustomerList2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomerList2.Item1);
            userWithProductDetails2 = customerDetailsList2;

            //Delete customer details list
            Tuple<HttpStatusCode, String> responseDeleteCustomerList = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(customerDetailsList);
            logger.WriteLine("Response Delete Entitlement for many User if they can Access to Multi Product: " + responseDeleteCustomerList);
            Assert.Equal(expected: HttpStatusCode.OK, responseDeleteCustomerList.Item1);
        }

        [Fact]
        public void Test776077_Delete_entitlement_for_one_user_if_they_not_access_to_any_product()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //Delete User
            Tuple<HttpStatusCode, String> responseDeleteUserWithProduct = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(userWithProductDetails1);
            logger.WriteLine("Response Delete User with Product: " + responseDeleteUserWithProduct);
            Assert.Equal(expected: HttpStatusCode.OK, responseDeleteUserWithProduct.Item1);

            //Get available Solution Families for user by user email to verify User hasn't deleted.
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id($"email_{userName}@gmail.com", null);
            logger.WriteLine("Response Get User not access to any Product: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);

            //After step delete above, User already have customerId=null and products is an empty list. We continue to make another delete method.
            var deleteUserWithProductDetailsContinue = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", "", true);
            Tuple<HttpStatusCode, String> responseDeleteUserWithProductContinue = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(deleteUserWithProductDetailsContinue);
            logger.WriteLine("Response Delete User can not access to any Product: " + responseDeleteUserWithProductContinue);
            Assert.Equal(expected: HttpStatusCode.OK, responseDeleteUserWithProductContinue.Item1);

            //Get available Solution Families for user by user email to verify User hasn't deleted.
            Tuple<HttpStatusCode, String> responseGetUserContinue = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id($"email_{userName}@gmail.com", null);
            logger.WriteLine("Response Get User (Verify User hasn't deleted): " + responseGetUserContinue);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUserContinue.Item1);
        }

        [Fact]
        public void Test776083_Delete_Entitlements_For_Multi_Use_With_Valid_And_Invalid_Email()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userName = StringUtils.RandomString(10);
            string email = $"email_{userName}@gmail.com";
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, email, userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //create list information for customers need to create
            List<string> listEmail = new List<string> { email , "invalid email"};
            List<string> listDisplayName = new List<string> { null, null };
            List<bool> listLocalAccount = new List<bool> { true, true};
            //create customer details list with productId1
            var customerDetailsList = EntitlementFactory.createEntitlementWithMultipleUsers(productId1, customerId, listEmail, listDisplayName, listLocalAccount);
            //Delete User
            Tuple<HttpStatusCode, String> responseDeleteUserWithProduct = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(customerDetailsList);
            if (responseDeleteUserWithProduct.Item1 == HttpStatusCode.OK)
            {
                logger.WriteLine("Response Delete User with Product: " + responseDeleteUserWithProduct);
                Assert.Equal(expected: HttpStatusCode.OK, responseDeleteUserWithProduct.Item1);
            }
            else
            {
                logger.WriteLine("Response Delete User with Product: " + responseDeleteUserWithProduct);
                Assert.Equal(expected: HttpStatusCode.BadRequest, responseDeleteUserWithProduct.Item1);
            }
        }

        [Fact]
        public void Test778990_Get_solution_families_with_valid_email_entitlements_B2C_and_valid_customerId()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //Get available Solution Families for user by user email & customerId
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id($"email_{userName}@gmail.com", customerId);
            logger.WriteLine("Response Get Available Solution Families for User by User Email in B2C & customerId : " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);
        }

        [Fact]
        public void Test778992_Get_Solution_Families_With_Valid_Email_Entitlement_B2C_And_Invalid_CustomerID()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //Get available Solution Families with Valid User Email & Invalid customerId
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id($"email_{userName}@gmail.com", $"Invalid_{customerId}");
            logger.WriteLine("Response Get Available Solution Families with Valid User Email in B2C & Invalid customerId: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseGetUser.Item1);
        }

        [Fact]
        public void Test778998_Get_Solution_Families_With_Valid_Email_Entitlement_B2C_And_Blank_CustomerID_User_Access_To_Many_Product()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();
            Setup_2nd_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product (1)
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //Create User with product (2)
            string userName2 = StringUtils.RandomString(10);
            userWithProductDetails2 = EntitlementFactory.createEntitlement(productId2, customerId, $"email_{userName}@gmail.com", userName2, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct2 = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails2);
            logger.WriteLine("Response responseCreateuserWithProduct2: " + responseCreateuserWithProduct2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct2.Item1);

            //Get available Solution Families with Valid User Email & blank customerId
            Tuple< HttpStatusCode, String > responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id($"email_{userName}@gmail.com", null);
            logger.WriteLine("Response Get Available Solution Families with Valid User Email in B2C & blank customerId access to many Product: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);
        }

        [Fact]
        public void Test779000_Get_Solution_Families_With_Valid_Email_Entitlement_And_Valid_CustomerID()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, false);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //Get available Solution Families for user by user email & customerId
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id($"email_{userName}@gmail.com", customerId);
            logger.WriteLine("Response Get Available Solution Families for User by User Email in Entitlement & customerId : " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);
        }

        [Fact]
        public void Test779004_Get_Solution_Families_With_Valid_Email_Entitlement_And_Blank_CustomerID_User_Access_To_Many_Product()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();
            Setup_2nd_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product (1)
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, false);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //Create User with product (2)
            string userName2 = StringUtils.RandomString(10);
            userWithProductDetails2 = EntitlementFactory.createEntitlement(productId2, customerId, $"email_{userName}@gmail.com", userName2, false);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct2 = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails2);
            logger.WriteLine("Response Create User with product (2): " + responseCreateuserWithProduct2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct2.Item1);

            //Get available Solution Families with Valid User Email & blank customerId
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id($"email_{userName}@gmail.com", null);
            logger.WriteLine("Response Get Available Solution Families with Valid User Email in Entitlement & blank customerId access to many Product: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);
        }

        [Fact]
        public void Test779006_Get_Solution_Families_With_Valid_Email_Entitlement_And_Blank_CustomerID_User_Access_To_One_Product()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, false);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //Get available Solution Families with Valid User Email & blank customerId
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id($"email_{userName}@gmail.com", null);
            logger.WriteLine("Response Get Available Solution Families with Valid User Email in Entitlement & blank customerId access to one Product: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);
        }

        [Fact]
        public void Test779011_Get_Solution_Families_With_Valid_Email_B2C_And_Valid_CustomerID()
        {
            // Create new customer to get customerID
            customerId = $"{Guid.NewGuid()}";
            string customerName = $"Customer_{StringUtils.RandomString(10)}";
            var customerDetails = CustomerFactory.createCustomer(customerId, customerName, true);
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            //Create new B2C User
            string email = $"User{StringUtils.RandomString(10)}@gmail.com";
            var userDetails = IdentityFactory.createIdentity(email, email.Replace("@gmail.com", ""));
            Tuple<HttpStatusCode, String> responseCreateUser = IdentityAPIs.Create_B2C_User(userDetails);
            logger.WriteLine("Response Create B2C User: " + responseCreateUser.Item2);
            Assert.Equal(expected: HttpStatusCode.Created, responseCreateUser.Item1);

            //Get available Solution Families for user by user email & customerId
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id(email, customerId);
            logger.WriteLine("Response Get Available Solution Families with Valid User Email B2C and Valid customerId: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.NotFound, responseGetUser.Item1);
        }

        [Fact]
        public void Test779013_Get_Solution_Families_With_Valid_Email_B2C_And_Blank_CustomerID()
        {
            //Create new B2C User
            string email = $"User_{StringUtils.RandomString(10)}@gmail.com";
            var userDetails = IdentityFactory.createIdentity(email, email.Replace("@gmail.com", ""));
            Tuple<HttpStatusCode, String> responseCreateUser = IdentityAPIs.Create_B2C_User(userDetails);
            logger.WriteLine("Response Create B2C User: " + responseCreateUser.Item2);
            Assert.Equal(expected: HttpStatusCode.Created, responseCreateUser.Item1);

            //Get available Solution Families for user by user email & customerId
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id(email, null);
            logger.WriteLine("Response Get Available Solution Families with Valid User Email B2C and blank customerId: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.NotFound, responseGetUser.Item1);
        }

        [Fact]
        public void Test779015_Get_Solution_Families_With_Invalid_Email_And_Valid_CustomerID()
        {
            // Create new customer to get customerID
            customerId = $"{Guid.NewGuid()}";
            string customerName = $"Customer_{StringUtils.RandomString(10)}";
            var customerDetails = CustomerFactory.createCustomer(customerId, customerName, true);
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            //Get available Solution Families for user by user email & customerId
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id($"Invalid_Email_{StringUtils.RandomString(10)}@gmail.com", customerId);
            logger.WriteLine("Response Get Available Solution Families with Inalid User Email B2C and Valid customerId: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.NotFound, responseGetUser.Item1);
        }

        [Fact]
        public void Test779023_Get_Solution_Families_With_Blank_Email_And_Blank_CustomerID()
        {
            //Get available Solution Families for user by user email & customerId
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id(null, null);
            logger.WriteLine("Response Get Available Solution Families with blank User Email B2C and Valid customerId: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseGetUser.Item1);
        }

        [Fact]
        public void Test775662_Create_Entitlements_Multi_User()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();
           
            //create list information for customers need to create
            List<string> listEmail = new List<string>();
            List<string> listDisplayName = new List<string>();
            List<bool> listLocalAccount = new List<bool>();
            //add information of customers to list
            for (int i=0; i < 5; i++)
            {
                listEmail.Add($"Customer_{StringUtils.RandomString(10)}@gmail.com");
                listDisplayName.Add($"Customer_{StringUtils.RandomString(10)}");
                listLocalAccount.Add(new Random().Next(10) < 5);
            }

            //create customer details list
            var customerDetailsList = EntitlementFactory.createEntitlementWithMultipleUsers(productId1, customerId, listEmail, listDisplayName, listLocalAccount);
            Tuple<HttpStatusCode, String> responseCreateCustomerList = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(customerDetailsList);
            logger.WriteLine("Response Create Customer Details List: " + responseCreateCustomerList);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomerList.Item1);
            userWithProductDetails1 = customerDetailsList;
        }
        [Fact]
        public void Test_776062_Delete_Entitlements_For_One_User_With_Valid_Value()
        {
            //Run the Setup function to get customerId and productId1
            Setup_1st_ProductEnvironment_To_Create_User_with_Product();

            //Create User with product
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //Delete User
            Tuple<HttpStatusCode, String> responseDeleteUserWithProduct = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(userWithProductDetails1);
            logger.WriteLine("Response Delete User with Product: " + responseDeleteUserWithProduct);
            Assert.Equal(expected: HttpStatusCode.OK, responseDeleteUserWithProduct.Item1);

            //Get available Solution Families for user by user email to verify Product is deleted
            Tuple<HttpStatusCode, String> responseGetUser = EntitlementAPIs.Get_Available_Solution_Families_For_User_By_Email_and_Customer_Id($"email_{userName}@gmail.com", null);
            logger.WriteLine("Response Get User (Verify User Product is deleted): " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);
        }

        [Fact]
        public void Test776064_Delete_Entitlements_Multi_User()
        {
            // Initialze the step to get the valid ecustomer details list from database
            Test775662_Create_Entitlements_Multi_User();

            //create customer details list
            Tuple<HttpStatusCode, String> responseCreateCustomerList = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomerList.Item1);
            //Delete customer details list
            Tuple<HttpStatusCode, String> responseDeleteCustomerList = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(userWithProductDetails1);
            logger.WriteLine("Response Delete Customer Details List: " + responseDeleteCustomerList);
            Assert.Equal(expected: HttpStatusCode.OK, responseDeleteCustomerList.Item1);
        }

        [Fact]
        public void Test776072_Delete_Entitlements_For_One_User_With_Invalid_CompanyId_And_ProductId()
        {
            //Delete customer details list
            var customerDetailsList = EntitlementFactory.createEntitlement($"Invalid_{Guid.NewGuid()}", $"Invalid_{Guid.NewGuid()}", $"Customer_{StringUtils.RandomString(10)}@gmail.com", null, true);
            Tuple<HttpStatusCode, String> responseDeleteCustomerList = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(customerDetailsList);
            logger.WriteLine("Response Delete Entitlements with Invalid customerId and productId: " + responseDeleteCustomerList.Item2);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseDeleteCustomerList.Item1);
        }

        [Fact]
        public void Test776938_Delete_Entitilement_Without_ProductID_CustomerId()
        {
            var customerDelete = new Entitlement
            {
                users = new List<User>()
                {
                    UserFactory.createUser($"Customer_{StringUtils.RandomString(10)}@gmail.com")
                }
            };
            //Delete Entitlement without ProductId and CustomerId
            Tuple<HttpStatusCode, String> responseCreateCustomerList = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(customerDelete);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateCustomerList.Item1);
            logger.WriteLine("Response Delete Entitlement without ProductId and CustomerId: " + responseCreateCustomerList);
            Assert.Equal(expected: "The ProductId field is required.", JObject.Parse(responseCreateCustomerList.Item2)["ProductId"][0]);
            Assert.Equal(expected: "The CustomerId field is required.", JObject.Parse(responseCreateCustomerList.Item2)["CustomerId"][0]);
        }



        //Setup 2 Product Environment function
        public void Setup_1st_ProductEnvironment_To_Create_User_with_Product()
        {
            //Create new productLine
            productLineId = $"{Guid.NewGuid()}";
            productLineName = $"ProductLine_{StringUtils.RandomString(10)}";
            var productLineDetails = ProductLineFactory.createProductLine(productLineId, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProductLine = ProductLineAPIs.Create_New_ProductLine(productLineDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductLine.Item1);

            // Create new product(1)
            productId1 = $"{Guid.NewGuid()}";
            string productName1 = $"Product_{StringUtils.RandomString(10)}";
            string productCode1 = StringUtils.RandomString(5);
            var productDetails1 = ProductFactory.createProductList(productId1, productName1, productCode1, productIconColor, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProduct1 = ProductAPIs.Create_Products(productDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProduct1.Item1);

            // Create new product environment(1)
            productEnvironmentId1 = $"{Guid.NewGuid()}";
            string productEnvironmentName1 = $"Product_Environment_{StringUtils.RandomString(10)}";
            string urlProductEnvironment1 = $"https://testAPI.com/{productEnvironmentId1}";
            string productEnvironmentCode1 = StringUtils.RandomString(5);
            var productEnvironmentDetails1 = EnvironmentFactory.createEnvironment(productEnvironmentId1, productId1, productEnvironmentName1, urlProductEnvironment1, productEnvironmentCode1);
            Tuple<HttpStatusCode, String> responseCreateProductEnvironment1 = EnvironmentAPIs.Create_New_Product_Environment(productEnvironmentDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductEnvironment1.Item1);

            // Create new customer to get customerID
            customerId = $"{Guid.NewGuid()}";
            string customerName = $"Customer_{StringUtils.RandomString(10)}";
            var customerDetails = CustomerFactory.createCustomer(customerId, customerName, true);;
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            //Associate Customer And Product Environment(1)
            associateCustomerDetails = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer1 = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId1, associateCustomerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer1.Item1);
        }

        public void Setup_2nd_ProductEnvironment_To_Create_User_with_Product()
        {
            // Create new product(2)
            productId2 = $"{Guid.NewGuid()}";
            string productName2 = $"Product_{StringUtils.RandomString(10)}";
            string productCode2 = StringUtils.RandomString(5);
            var productDetails2 = ProductFactory.createProductList(productId2, productName2, productCode2, productIconColor, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProduct2 = ProductAPIs.Create_Products(productDetails2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProduct2.Item1);

            // Create new product environment(2)
            productEnvironmentId2 = $"{Guid.NewGuid()}";
            string productEnvironmentName2 = $"Product_Environment_{StringUtils.RandomString(10)}";
            string urlProductEnvironment2 = $"https://testAPI.com/{productEnvironmentId2}";
            string productEnvironmentCode2 = StringUtils.RandomString(5);
            var productEnvironmentDetails2 = EnvironmentFactory.createEnvironment(productEnvironmentId2, productId2, productEnvironmentName2, urlProductEnvironment2, productEnvironmentCode2);
            Tuple<HttpStatusCode, String> responseCreateProductEnvironment2 = EnvironmentAPIs.Create_New_Product_Environment(productEnvironmentDetails2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductEnvironment2.Item1);

            //Associate Customer And Product Environment(2)
            Tuple<HttpStatusCode, String> responseAssociateCustomer2 = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId2, associateCustomerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer2.Item1);
        }
    }
}