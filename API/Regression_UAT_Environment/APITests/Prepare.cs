using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Regression_UAT_Environment.Utils;
using System.Net;
using Xunit;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Regression_UAT_Environment.Tests
{
    public class Prepare : IDisposable
    {
        private ITestOutputHelper logger;
        private string productLineId;
        private string productLineName;
        private string productId;
        private string productIconColor = String.Format("#{0:X6}", new Random().Next(0x1000000));
        private string productEnvironmentId1;
        private string customerId;
        private string hierarchyId;
        private Associate associateCustomerDetails;
        private Entitlement userWithProductDetails1;
        private Hierarchy hierarchyDetails;
        private Role roleDetails;

        public Prepare(ITestOutputHelper logger)
        {
            this.logger = logger;
        }
        public void Dispose()
        {
        }

        [Fact]
        public void Test_create_prod_and_cust_and_asociated()
        {
            //Create new productLine
            productLineId = $"{Guid.NewGuid()}";
            productLineName = $"ProductLine_{StringUtils.RandomString(10)}";
            var productLineDetails = ProductLineFactory.createProductLine(productLineId, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProductLine = ProductLineAPIs.Create_New_ProductLine(productLineDetails);
            logger.WriteLine("New Product Line:" + responseCreateProductLine.Item2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductLine.Item1);

            // Create new product(1)
            productId = $"{Guid.NewGuid()}";
            string productName1 = $"Product_{StringUtils.RandomString(10)}";
            string productCode1 = StringUtils.RandomString(5);
            var productDetails1 = ProductFactory.createProductList(productId, productName1, productCode1, productIconColor, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProduct1 = ProductAPIs.Create_Products(productDetails1);
            logger.WriteLine("New Product:" + responseCreateProduct1.Item2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProduct1.Item1);

            // Create new product environment(1)
            productEnvironmentId1 = $"{Guid.NewGuid()}";

            string productEnvironmentName1 = $"Product_Environment_{StringUtils.RandomString(10)}";
            string urlProductEnvironment1 = $"https://testAPI.com/{productEnvironmentId1}";
            string productEnvironmentCode1 = StringUtils.RandomString(5);
            var productEnvironmentDetails1 = EnvironmentFactory.createEnvironment(productEnvironmentId1, productId, productEnvironmentName1, urlProductEnvironment1, productEnvironmentCode1);
            Tuple<HttpStatusCode, String> responseCreateProductEnvironment1 = EnvironmentAPIs.Create_New_Product_Environment(productEnvironmentDetails1);
            logger.WriteLine("New Product environment:" + responseCreateProductEnvironment1.Item2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductEnvironment1.Item1);

            // Create new customer
            customerId = $"{Guid.NewGuid()}";
            string customerName = $"Customer_{StringUtils.RandomString(10)}";
            var customerDetails = CustomerFactory.createCustomer(customerId, customerName, true); ;
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            logger.WriteLine("New Customer:" + responseCreateCustomer.Item2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            //Associate Customer And Product Environment(1)
            associateCustomerDetails = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer1 = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId1, associateCustomerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer1.Item1);

            ////Create User with customer
            //string userName = StringUtils.RandomString(10);
            //userWithProductDetails1 = EntitlementFactory.createEntitlement(productId1, customerId, $"email_{userName}@gmail.com", userName, false);
            //Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            //Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            ////Check the default customer is the first customer (in the list customer of User)
            //Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email($"email_{userName}@gmail.com");
            //logger.WriteLine("Response Get User with Invalid Email: " + responseGetUser);
            //Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);
        }

        [Fact]
        public void Test_create_user_with_prod_and_cust()
        {
            //Create new productLine
            productLineId = $"{Guid.NewGuid()}";
            productLineName = $"ProductLine_{StringUtils.RandomString(10)}";
            var productLineDetails = ProductLineFactory.createProductLine(productLineId, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProductLine = ProductLineAPIs.Create_New_ProductLine(productLineDetails);
            logger.WriteLine("New Product Line:" + responseCreateProductLine.Item2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductLine.Item1);

            // Create new product(1)
            productId = $"{Guid.NewGuid()}";
            string productName1 = $"Product_{StringUtils.RandomString(10)}";
            string productCode1 = StringUtils.RandomString(5);
            var productDetails1 = ProductFactory.createProductList(productId, productName1, productCode1, productIconColor, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProduct1 = ProductAPIs.Create_Products(productDetails1);
            logger.WriteLine("New Product:" + responseCreateProduct1.Item2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProduct1.Item1);

            // Create new product environment(1)
            productEnvironmentId1 = $"{Guid.NewGuid()}";

            string productEnvironmentName1 = $"Product_Environment_{StringUtils.RandomString(10)}";
            string urlProductEnvironment1 = $"https://testAPI.com/{productEnvironmentId1}";
            string productEnvironmentCode1 = StringUtils.RandomString(5);
            var productEnvironmentDetails1 = EnvironmentFactory.createEnvironment(productEnvironmentId1, productId, productEnvironmentName1, urlProductEnvironment1, productEnvironmentCode1);
            Tuple<HttpStatusCode, String> responseCreateProductEnvironment1 = EnvironmentAPIs.Create_New_Product_Environment(productEnvironmentDetails1);
            logger.WriteLine("New Product environment:" + responseCreateProductEnvironment1.Item2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductEnvironment1.Item1);

            // Create new customer
            customerId = $"{Guid.NewGuid()}";
            string customerName = $"Customer_{StringUtils.RandomString(10)}";
            var customerDetails = CustomerFactory.createCustomer(customerId, customerName, true); ;
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            logger.WriteLine("New Customer:" + responseCreateCustomer.Item2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            //Associate Customer And Product Environment(1)
            associateCustomerDetails = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer1 = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId1, associateCustomerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer1.Item1);

            //Create User with customer
            string userName = StringUtils.RandomString(10);
            userWithProductDetails1 = EntitlementFactory.createEntitlement(productId, customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails1);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //Check the default customer is the first customer (in the list customer of User)
            Tuple<HttpStatusCode, String> responseGetUser = UserAPIs.Get_User_By_Email($"email_{userName}@gmail.com");
            logger.WriteLine("Response Get User with Invalid Email: " + responseGetUser);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetUser.Item1);
        }

        [Fact]
        public void Test_create_hierarchy()
        {
            Test_create_user_with_prod_and_cust();

            //Create new Hierarchy
            string hierarchyName = $"Hierarchy_{StringUtils.RandomString(10)}";
            hierarchyDetails = HierarchyFactory.createHierarchy(hierarchyName, productEnvironmentId1, customerId);
            Tuple<HttpStatusCode, String> responseCreateHierarchy = HierarchyAPIs.Create_Hierarchy(hierarchyDetails);
            logger.WriteLine("New Hierarchy:" + responseCreateHierarchy.Item2);
            hierarchyId = JObject.Parse(responseCreateHierarchy.Item2)["id"].ToString();
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateHierarchy.Item1);
        }

        [Fact]
        public void Test_create_role()
        {
            Test_create_hierarchy();
            string roleName = $"Role_{StringUtils.RandomString(10)}";
            string sfRoleId = $"sfRoleId_{StringUtils.RandomString(10)}";
            roleDetails = RoleFactory.createRole(customerId, productId, roleName, sfRoleId, hierarchyId);
            Tuple<HttpStatusCode, String> responseCreateRole = RoleAPIs.Create_Role(roleDetails);
            logger.WriteLine("New Role:" + responseCreateRole.Item2);
        }
    }
}
