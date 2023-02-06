using Xunit;
using System.Net;
using Xunit.Abstractions;
using System;
using Newtonsoft.Json.Linq;
using Regression_UAT_Environment.Utils;
using System.Text.RegularExpressions;

namespace Regression_UAT_Environment.Tests
{
    public class CustomerTests : IDisposable
    {
        // Declare the variables
        private readonly ITestOutputHelper logger;
        private string customerId1;
        private string customerId2;

        public CustomerTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        //Teardown
        public void Dispose()
        {
            //Cleanup
            //Cleanup Customer (1)
            if (customerId1 != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteCustomer1 = CustomerAPIs.Delete_Customers_Details(customerId1);
            }
            //Cleanup Customer (2)
            if (customerId2 != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteCustomer2 = CustomerAPIs.Delete_Customers_Details(customerId2);
            }
            logger.WriteLine("Successfully Cleanup the Customer Tests");
        }

        [Fact]
        public void Test776105_Get_Customers()
        {
            Tuple<HttpStatusCode, String> responseGetListCustomers = CustomerAPIs.Get_All_Customers_Details();
            logger.WriteLine("Response Get Customer: " + responseGetListCustomers);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetListCustomers.Item1);
        }

        [Fact]
        public void Test777843_Get_User_With_Valid_CustomerID()
        {
            //Get a list of customers available in entitlements.
            Tuple<HttpStatusCode, String> responseGetListCustomers = CustomerAPIs.Get_All_Customers_Details();
            Assert.Equal(expected: HttpStatusCode.OK, responseGetListCustomers.Item1);
            JArray allCustomer = JArray.Parse(responseGetListCustomers.Item2);
            string customerId = allCustomer[new Random().Next(0, allCustomer.Count - 1)]["id"].ToString();

            //Get User with valid customerId
            Tuple<HttpStatusCode, String> responseGetCustomer = CustomerAPIs.Get_Customers_Details(customerId);
            logger.WriteLine("Response Get Customer with Valid customerId:" + responseGetCustomer);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetCustomer.Item1);
            Assert.Equal(expected: customerId, JObject.Parse(responseGetCustomer.Item2)["id"].ToString());
        }

        [Fact]
        public void Test777844_Get_Customer_With_Invalid_ID()
        {
            string customerId = $"{Guid.NewGuid()}";
            string name = "name_test_" + StringUtils.RandomString(10);
            //Create new user
            var customer = CustomerFactory.createCustomer(customerId, name, false);
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customer);
            logger.WriteLine("Response Create User: " + responseCreateCustomer);
            customerId1 = customerId;

            Tuple<HttpStatusCode, String> responseGetCustomersDetails = CustomerAPIs.Get_Customers_Details(customerId);
            logger.WriteLine("Response get Customer Details with Invalid ID: " + responseGetCustomersDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetCustomersDetails.Item1);
        }

        [Fact]
        public void Test777847_Delete_Customer_With_Valid_ID_Customer_Not_Have_Associate_With_Environment_Or_Product_Or_User()
        {
            string customerId = $"{Guid.NewGuid()}";
            string name = "name_test_" + StringUtils.RandomString(10);
            //Create new user
            var customer = CustomerFactory.createCustomer(customerId, name, false);
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customer);
            logger.WriteLine("Response Create User: " + responseCreateCustomer);
            //Delete user
            Tuple<HttpStatusCode, String> responseDeleteCustomer = CustomerAPIs.Delete_Customers_Details(customerId);
            logger.WriteLine("Response Delete User: " + responseDeleteCustomer);
            Assert.Equal(expected: HttpStatusCode.OK, responseDeleteCustomer.Item1);
        }

        [Fact]
        public void Test777848_Delete_Customer_With_Non_Exist_ID()
        {
            string customerId = $"{Guid.NewGuid()}";
            Tuple<HttpStatusCode, String> responseDeleteCustomer = CustomerAPIs.Delete_Customers_Details(customerId);
            logger.WriteLine("ResponseDelete non Exist User): " + responseDeleteCustomer);
            Assert.Equal(expected: HttpStatusCode.NotFound, responseDeleteCustomer.Item1);
        }

        [Fact]
        public void Test777851_Update_Customer_Details_With_Valid_ID_And_Request_Body()
        {
            string customerId = $"{Guid.NewGuid()}";
            string name = "name_test_" + StringUtils.RandomString(10);
            //Create new user
            var customer = CustomerFactory.createCustomer(customerId, name, false);
            Tuple<HttpStatusCode, String> response = CustomerAPIs.Create_Customers_Details(customer);
            logger.WriteLine("Response Create User: " + response);
            customerId1 = customerId;

            //Update User details
            string nameChanged = $"{name}_changed";
            var customerUpdate = CustomerFactory.createCustomer(customerId, nameChanged, false);
            Tuple<HttpStatusCode, String> responseUpdate = CustomerAPIs.Update_Customers_Details(customerId, customerUpdate);
            Assert.Equal(expected: HttpStatusCode.OK, responseUpdate.Item1);

            //Get user details to validate
            Tuple<HttpStatusCode, String> responseGet = CustomerAPIs.Get_Customers_Details(customerId);
            logger.WriteLine("Response update Customer: " + responseGet);
            Assert.Equal(expected: HttpStatusCode.OK, responseGet.Item1);
            Assert.Equal(expected: nameChanged, JObject.Parse(responseGet.Item2)["name"]);
        }

        [Fact]
        public void Test777852_Update_Customer_Details_With_Non_Exist_ID_And_Valid_Request_Body()
        {
            string customerId = $"{Guid.NewGuid()}";
            string name = "name_update_" + StringUtils.RandomString(10);
            var customer = CustomerFactory.createCustomer(customerId, name, false);
            Tuple<HttpStatusCode, String> responseUpdateCustomer = CustomerAPIs.Update_Customers_Details(customerId, customer);
            logger.WriteLine("Response Update non Exist User: " + responseUpdateCustomer);
            Assert.Equal(expected: HttpStatusCode.NotFound, responseUpdateCustomer.Item1);
        }

        //[Fact]
        public void Test777855_Update_Customer_Details_With_Valid_ID_And_Blank_Active_Status()
        {
            string customerId = $"{Guid.NewGuid()}";
            string name = "name_test_" + StringUtils.RandomString(10);
            //Create new user
            var customer = CustomerFactory.createCustomer(customerId, name, false);
            Tuple <HttpStatusCode, String> response = CustomerAPIs.Create_Customers_Details(customer);
            logger.WriteLine("Response Create User:" + response);
            customerId1 = customerId;

            //Update Customer with blank Active
            string nameUpdate = "name_update_" + StringUtils.RandomString(10);
            var customerWithBlankActive = CustomerFactory.createCustomer(customerId, nameUpdate);
            Tuple<HttpStatusCode, String> responseUpdateCustomer = CustomerAPIs.Update_Customers_Details(customerId, customerWithBlankActive);
            logger.WriteLine("Response Update Customer with blank Active: " + responseUpdateCustomer);
            Assert.Equal(expected: HttpStatusCode.OK, responseUpdateCustomer.Item1);
        }

        [Fact]
        public void Test777856_Update_customer_with_valid_ID_and_existing_name_of_another_customer()
        {
            string[] listId = { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" };
            string[] listName = { $"name_test_" + StringUtils.RandomString(10), "name_test_" + StringUtils.RandomString(10) };
            bool[] listActive = { false, false };
            //Create 2 new users
            for (int i = 0; i < listId.Length; i++)
            {
                var customer = CustomerFactory.createCustomer(listId[i], listName[i], listActive[i], $"{Guid.NewGuid()}", $"{Guid.NewGuid()}", $"ProductEnvironment_{StringUtils.RandomString(6)}", $"https://spheracloud_{StringUtils.RandomString(10)}.net", StringUtils.RandomString(6));
                Tuple<HttpStatusCode, String> responseCreate = CustomerAPIs.Create_Customers_Details(customer);
                Assert.Equal(expected: HttpStatusCode.OK, responseCreate.Item1);
            }
            customerId1 = listId[0];
            customerId2 = listId[1];
            var customerUpdate = CustomerFactory.createCustomer(listId[0], listName[1], listActive[0]);
            Tuple<HttpStatusCode, String> responseUpdate = CustomerAPIs.Update_Customers_Details(listId[0], customerUpdate);
            logger.WriteLine("Response Update Customer:" + responseUpdate);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdate.Item1);
            Assert.Equal(expected: "This customer already exists.", JObject.Parse(responseUpdate.Item2)["name"][0]);
        }

        [Fact]
        public void Test821727_Create_New_Customer_With_Invalid_ID_Valid_Name_Valid_Active_Status()
        {
            string customerId = $"ABC{Guid.NewGuid()}";
            String name = "name_test_" + StringUtils.RandomString(10);
            //Create new user
            var customer = CustomerFactory.createCustomer(customerId, name, true);
            Tuple<HttpStatusCode, String> responseCreate = CustomerAPIs.Create_Customers_Details(customer);
            logger.WriteLine("Response Create Customer with Invalid Id:" + responseCreate);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreate.Item1);
        }

        [Fact]
        public void Test821728_Create_New_Customer_With_Blank_ID_Valid_Name_Valid_Active_Status()
        {
            string customerid = null;
            String name = "name_test_" + StringUtils.RandomString(10);
            //Create new user
            var customerWithBlankID = CustomerFactory.createCustomer(customerid, name, true);
            Tuple<HttpStatusCode, String> responseCreate = CustomerAPIs.Create_Customers_Details(customerWithBlankID);
            logger.WriteLine("Response Create Customer with Blank Id:" + responseCreate);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreate.Item1);
            Assert.NotNull(JObject.Parse(responseCreate.Item2)["id"].ToString());
            string ID = JObject.Parse(responseCreate.Item2)["id"].ToString();
            Regex _isGuid = new Regex(@"[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?", RegexOptions.Compiled);
            Assert.True(_isGuid.IsMatch(ID), $"{ID} is NOT a GUID format.");
            customerId1 = ID;
        }
    }
}