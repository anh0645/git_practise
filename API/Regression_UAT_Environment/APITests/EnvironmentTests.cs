using Xunit;
using System.Net;
using Xunit.Abstractions;
using System;
using Regression_UAT_Environment.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Regression_UAT_Environment.Tests
{
    public class EnvironmentTests : IDisposable
    {
        // Declare the variables
        private readonly ITestOutputHelper logger;
        private string customerId;
        private string productLineId;
        private string productLineId2;
        private string productId;
        private string productId2;
        private string productEnvironmentId;
        private string productEnvironmentId2;
        private string productEnvironmentName;
        private string urlProductEnvironment;
        private string productEnvironmentCode;
        private string productIconColor = String.Format("#{0:X6}", new Random().Next(0x1000000));
        private string solutionFamilyId;
        private string solutionFamilyName;
        private string solutionFamilyCode;
        private SolutionFamily solutionFamilyDetails;
        private Associate associateCustomerDetails;
        private Associate associateCustomerDetails2;
        private Environment environmentDetails;
        private Entitlement userWithProductDetails;

        // Initialize the varible using in script
        public EnvironmentTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        //Teardown
        public void Dispose()
        {
            //Cleanup
            //if (userWithProductDetails != null)
            //{
            //    Tuple<HttpStatusCode, String> responseDeleteUserWithProduct = EntitlementAPIs.Delete_Access_From_Users_For_The_Specificed_Customer_Product_Pair(userWithProductDetails);
            //}
            //if (associateCustomerDetails != null)
            //{
            //    //Disassociate Customer And Product Environment
            //    Tuple<HttpStatusCode, String> responseDisassociateCustomer = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment(productEnvironmentId, customerId);
            //}
            //if (associateCustomerDetails2 != null)
            //{
            //    //Disassociate Customer And Product Environment
            //    Tuple<HttpStatusCode, String> responseDisassociateCustomer2 = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment(productEnvironmentId2, customerId);
            //}
            ////Cleanup ProductEnv
            //if (productEnvironmentId != null)
            //{
            //    Tuple<HttpStatusCode, String> responseDeleteProductEnvironment1 = EnvironmentAPIs.Delete_Environment_From_Environments_By_ID(productEnvironmentId);
            //}
            //if (productEnvironmentId2 != null)
            //{
            //    Tuple<HttpStatusCode, String> responseDeleteProductEnvironment2 = EnvironmentAPIs.Delete_Environment_From_Environments_By_ID(productEnvironmentId2);
            //}
            ////Cleanup Product
            //if (productId != null)
            //{
            //    Tuple<HttpStatusCode, String> responseDeleteProduct = ProductAPIs.Delete_Product_From_Entitlements_By_Id(productId);
            //}
            //if (productId2 != null)
            //{
            //    Tuple<HttpStatusCode, String> responseDeleteProduct2 = ProductAPIs.Delete_Product_From_Entitlements_By_Id(productId2);
            //}
            ////Cleanup ProductLine
            //if (productLineId != null)
            //{
            //    Tuple<HttpStatusCode, String> responseDeleteProductLine = ProductLineAPIs.Delete_ProductLine_By_Id(productLineId);
            //}
            //if (productLineId2 != null)
            //{
            //    Tuple<HttpStatusCode, String> responseDeleteProductLine2 = ProductLineAPIs.Delete_ProductLine_By_Id(productLineId2);
            //}
            ////Cleanup Customer
            //if (customerId != null)
            //{
            //    Tuple<HttpStatusCode, String> responseDeleteCustomer = CustomerAPIs.Delete_Customers_Details(customerId);
            //}
            //logger.WriteLine("Successfully Cleanup the Environment Tests");
        }

        [Fact]
        public void Test786313_Get_All_Environments()
        {
            //Get all Environments
            Tuple<HttpStatusCode, String> responseGetEnvironmentList = EnvironmentAPIs.Get_All_Environments();
            logger.WriteLine("Response Get all Environments: " + responseGetEnvironmentList);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetEnvironmentList.Item1);
            //Get random environmentId
            JArray jArray = JArray.Parse(responseGetEnvironmentList.Item2);
            int indexGetId = new Random().Next(0, jArray.Count - 1);
        }

        [Fact]
        public void Test786314_Get_All_Environments_with_Valid_ProductId()
        {
            Create_New_Product();
            Create_New_ProductEnvironment();
            Tuple<HttpStatusCode, String> responseGetEnvironmentList = EnvironmentAPIs.Get_All_Environments_with_ProductID(productId);
            logger.WriteLine("Response Get all Environments with Valid productID: " + responseGetEnvironmentList);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetEnvironmentList.Item1);
        }

        [Fact]
        public void Test786316_Get_All_Environments_with_Invalid_ProductId()
        {
            Tuple<HttpStatusCode, String> responseGetEnvironmentList = EnvironmentAPIs.Get_All_Environments_with_ProductID($"Invalid_{Guid.NewGuid()}");
            logger.WriteLine("Response Get all Environments with Invalid productID: " + responseGetEnvironmentList);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseGetEnvironmentList.Item1);
        }

        [Fact]
        public void Test786319_Delete_Environment_with_Valid_GUID()
        {
            // Initialze the step to get the valid environment ID from database
            ////Test786976_Create_New_Product_Environment_with_All_Valid_Value_in_RequestBody();
            Create_New_Product();
            Create_New_ProductEnvironment();
            logger.WriteLine(productEnvironmentId);

            //Delete Product Environment
            Tuple<HttpStatusCode, String> responseDeletenvironment = EnvironmentAPIs.Delete_Environment_From_Environments_By_ID(productEnvironmentId);
            logger.WriteLine("Response Delete Product Environment: " + responseDeletenvironment);
            Assert.Equal(expected: HttpStatusCode.OK, responseDeletenvironment.Item1);
        }

        [Fact]
        public void Test786321_Delete_Environment_with_Non_Existing_GUID()
        {
            //Delete Product Environment with non Existing GUID
            Tuple<HttpStatusCode, String> responseDeleteEnvironment = EnvironmentAPIs.Delete_Environment_From_Environments_By_ID($"{Guid.NewGuid()}");
            logger.WriteLine("Response Delete Product Environment with non Existing ID: " + responseDeleteEnvironment);
            Assert.Equal(expected: HttpStatusCode.NotFound, responseDeleteEnvironment.Item1);
        }

        [Fact]
        public void Test786976_Create_New_Product_Environment_with_All_Valid_Value_in_RequestBody()
        {
            // Initialze the step to create a new product and get productId
            Create_New_Product();

            //Create new Product Environment with Invalid Id
            productEnvironmentId = $"{Guid.NewGuid()}";
            environmentDetails = EnvironmentFactory.createEnvironment(productEnvironmentId, productId, $"Environment_{StringUtils.RandomString(10)}", $"https://{StringUtils.RandomString(10)}.spheracloud.net", StringUtils.RandomString(10));
            Tuple<HttpStatusCode, String> responseCreateEnvironment = EnvironmentAPIs.Create_New_Product_Environment(environmentDetails);
            logger.WriteLine("Response Create new Product Environment: " + responseCreateEnvironment);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateEnvironment.Item1);
        }

        [Fact]
        public void Test786977_Create_New_Product_Environment_With_Invalid_ID()
        {
            // Initialze the step to create a new product and get productId
            Create_New_Product();

            //Create new Product Environment with Invalid Id
            environmentDetails = EnvironmentFactory.createEnvironment($"!{Guid.NewGuid()}", productId, $"Environment_{StringUtils.RandomString(10)}", $"https://{StringUtils.RandomString(10)}.spheracloud.net", StringUtils.RandomString(10));
            Tuple<HttpStatusCode, String> responseCreateEnvironment = EnvironmentAPIs.Create_New_Product_Environment(environmentDetails);
            logger.WriteLine("Response Create new Product Environment with Invalid Id: " + responseCreateEnvironment);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateEnvironment.Item1);
            Assert.Equal(expected: "The field Id is invalid.", JObject.Parse(responseCreateEnvironment.Item2)["Id"][0].ToString());
        }

        [Fact]
        public void Test786984_Create_New_Product_Environment_With_Blank_ID()
        {
            // Initialze the step to create a new product and get productId
            Create_New_Product();

            //Create new Product Environment with Blank Id
            environmentDetails = EnvironmentFactory.createEnvironment(null, productId, $"Environment_{StringUtils.RandomString(10)}", $"https://{StringUtils.RandomString(10)}.spheracloud.net", StringUtils.RandomString(10));
            Tuple<HttpStatusCode, String> responseCreateEnvironment = EnvironmentAPIs.Create_New_Product_Environment(environmentDetails);
            logger.WriteLine("Response Create new Product Environment with Blank Id: " + responseCreateEnvironment);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateEnvironment.Item1);
            productEnvironmentId = JObject.Parse(responseCreateEnvironment.Item2)["id"].ToString();
        }

        [Fact]
        public void Test786992_Create_New_Product_Environment_with_Blank_ProductId_and_Another_Field_Is_Correct()
        {
            //Create new Product Environment with Invalid Id
            productEnvironmentId = $"{Guid.NewGuid()}";
            environmentDetails = EnvironmentFactory.createEnvironment(productEnvironmentId, "", $"Environment_{StringUtils.RandomString(10)}", $"https://{StringUtils.RandomString(10)}.spheracloud.net", StringUtils.RandomString(10));
            Tuple<HttpStatusCode, String> responseCreateEnvironment = EnvironmentAPIs.Create_New_Product_Environment(environmentDetails);
            logger.WriteLine("Response Create new Product Environment with blank productId: " + responseCreateEnvironment);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateEnvironment.Item1);
        }

        [Fact]
        public void Test786997_Create_New_Product_Environment_with_Name_Match_with_Another_Environment()
        {
            // Initialze the step to create a new product and get productName
            Test786976_Create_New_Product_Environment_with_All_Valid_Value_in_RequestBody();

            // Create a new product with the same productName
            productEnvironmentId2 = $"{Guid.NewGuid()}";
            var environmentDetails2 = EnvironmentFactory.createEnvironment(productEnvironmentId2, productId, environmentDetails.name, $"https://{StringUtils.RandomString(10)}.spheracloud.net", StringUtils.RandomString(10));
            Tuple<HttpStatusCode, String> responseCreateEnvironment = EnvironmentAPIs.Create_New_Product_Environment(environmentDetails2);
            logger.WriteLine("Response Create new Product Environment with productName match with another Environment: " + responseCreateEnvironment);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateEnvironment.Item1);
        }

        [Fact]
        public void Test787501_Create_New_Product_Environment_with_URL_Match_with_Another_Environment()
        {
            // Initialze the step to create a new product and get productName
            Test786976_Create_New_Product_Environment_with_All_Valid_Value_in_RequestBody();

            // Create a new product with the same productName
            productEnvironmentId2 = $"{Guid.NewGuid()}";
            var environmentDetails2 = EnvironmentFactory.createEnvironment(productEnvironmentId2, productId, $"Environment_{StringUtils.RandomString(10)}", environmentDetails.url, StringUtils.RandomString(10));
            Tuple<HttpStatusCode, String> responseCreateEnvironment = EnvironmentAPIs.Create_New_Product_Environment(environmentDetails2);
            logger.WriteLine("Response Create new Product Environment with productURL match with another Environment: " + responseCreateEnvironment);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateEnvironment.Item1);
        }

        [Fact]
        public void Test787538_Update_Environment_with_ValidGUID_and_Valid_RequestBody()
        {
            // Initialze the step to create a new product and get productId
            Create_New_Product();
            Create_New_ProductEnvironment();

            //Update Environment with Valid Environment Id and valid RequestBody
            var environmentUpdate = EnvironmentFactory.createEnvironment(null, null, $"Environment_{StringUtils.RandomString(10)}", $"https://{StringUtils.RandomString(10)}.spheracloud.net", StringUtils.RandomString(10));
            Tuple<HttpStatusCode, String> responseUpdateEnvironment = EnvironmentAPIs.Update_Product_Environment_Details(productEnvironmentId, environmentUpdate);
            logger.WriteLine("Response Update Environment with Valid productEnvironmentID and RequestBody: " + responseUpdateEnvironment);
            Assert.Equal(expected: HttpStatusCode.OK, responseUpdateEnvironment.Item1);
        }

        [Fact]
        public void Test787539_Update_Product_Environment_with_Invalid_GUID_and_Valid_RequestBody()
        {
            //Update Environment with Valid Environment Id and valid RequestBody
            var environmentUpdate = EnvironmentFactory.createEnvironment(null, null, $"Environment_{StringUtils.RandomString(10)}", $"https://{StringUtils.RandomString(10)}.spheracloud.net", StringUtils.RandomString(10));
            Tuple<HttpStatusCode, String> responseUpdateEnvironment = EnvironmentAPIs.Update_Product_Environment_Details($"Invalid_{Guid.NewGuid()}", environmentUpdate);
            logger.WriteLine("Response Update Environment with Invalid productEnvironmentID and Valid RequestBody: " + responseUpdateEnvironment);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateEnvironment.Item1);
        }

        //[Fact]
        public void Test787543_Update_Product_Environment_with_Valid_GUID_and_Blank_Name_or_URL_or_Code()
        {
            // Initialze the step to create a new product and get productId
            Create_New_Product();
            Create_New_ProductEnvironment();

            //Update Environment with Valid Environment Id and valid RequestBody
            var environmentUpdate = EnvironmentFactory.createEnvironment(null, null, null, $"https://{StringUtils.RandomString(10)}.spheracloud.net", StringUtils.RandomString(10));
            Tuple<HttpStatusCode, String> responseUpdateEnvironment = EnvironmentAPIs.Update_Product_Environment_Details(productEnvironmentId, environmentUpdate);
            logger.WriteLine("Response Update Environment with Valid productEnvironmentID and blank productEnvironmentName: " + responseUpdateEnvironment);
            Assert.Equal(expected: HttpStatusCode.OK, responseUpdateEnvironment.Item1);
        }

        [Fact]
        public void Test787545_Update_Product_Environment_with_Name_Match_with_Another_Environment()
        {
            // Initialze the step to create a new product and get productEnvironmentId
            Test786976_Create_New_Product_Environment_with_All_Valid_Value_in_RequestBody();

            // Create a new productEnvironment (2)
            productEnvironmentId2 = $"{Guid.NewGuid()}";
            var environmentDetails2 = EnvironmentFactory.createEnvironment(productEnvironmentId2, productId, $"Environment_{StringUtils.RandomString(10)}", $"https://{StringUtils.RandomString(10)}.spheracloud.net", StringUtils.RandomString(10));
            Tuple<HttpStatusCode, String> responseCreateEnvironment2 = EnvironmentAPIs.Create_New_Product_Environment(environmentDetails2);
            logger.WriteLine("Response Create new Product Environment (2): " + responseCreateEnvironment2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateEnvironment2.Item1);

            //Update Environment with Valid Environment Id and valid RequestBody
            var environmentUpdate = EnvironmentFactory.createEnvironment(null, null, environmentDetails.name, $"https://{StringUtils.RandomString(10)}.spheracloud.net", StringUtils.RandomString(10));
            Tuple<HttpStatusCode, String> responseUpdateEnvironment = EnvironmentAPIs.Update_Product_Environment_Details(productEnvironmentId2, environmentUpdate);
            logger.WriteLine("Response Update Environment with Valid productEnvironmentName math with another Environment: " + responseUpdateEnvironment);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateEnvironment.Item1);
        }

        [Fact]
        public void Test788206_Update_Product_Environment_with_Code_More_Than_12_Characters()
        {
            // Initialze the step to create a new product and get productEnvironmentId
            Test786976_Create_New_Product_Environment_with_All_Valid_Value_in_RequestBody();

            //Update Environment with Valid Environment Id and valid RequestBody
            var environmentUpdate = EnvironmentFactory.createEnvironment(null, null, environmentDetails.name, $"https://{StringUtils.RandomString(10)}.spheracloud.net", StringUtils.RandomString(14));
            Tuple<HttpStatusCode, String> responseUpdateEnvironment = EnvironmentAPIs.Update_Product_Environment_Details(productEnvironmentId, environmentUpdate);
            logger.WriteLine("Response Update Environment with productEnvironmentCode more than 12 Characters (14 Characters): " + responseUpdateEnvironment);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateEnvironment.Item1);
        }

        [Fact]
        public void Test790412_Create_Associate_Customer_And_Product_Environment_With_ValidGUID_And_RequestBody()
        {
            //Create new ProductEnvironment
            Create_New_Product();
            Create_New_ProductEnvironment();
            Create_New_Customer();

            //Associate Customer And Product Environment
            var associateCustomerDetails = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId, associateCustomerDetails);
            logger.WriteLine("Response Associate Customer: " + responseAssociateCustomer);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer.Item1);
        }

        [Fact]
        public void Test790413_Create_Associate_Customer_and_Product_Environment_with_Valid_GUID_Valid_Active_Status_and_Invalid_CustomerID()
        {
            //Create new ProductEnvironment
            Create_New_Product();
            Create_New_ProductEnvironment();

            //Create associate customer and product environment with invalid GUID and valid customerid and active status
            var associateCustomerDetails = AssociateFactory.createAssociate($"Invalid_{customerId}", true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId, associateCustomerDetails);
            logger.WriteLine("Response Associate Customer with Invalid customerID: " + responseAssociateCustomer);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseAssociateCustomer.Item1);
        }

        [Fact]
        public void Test790422_Create_Associate_Customer_and_Product_Environment_with_Valid_GUID_And_Blank_CustomerID_Active_Status()
        {
            //Create new ProductEnvironment
            Create_New_Product();
            Create_New_ProductEnvironment();

            //Create associate customer with Invalid customerID
            var associateCustomerDetails = AssociateFactory.createAssociate(null);
            Tuple<HttpStatusCode, String> responseAssociateCustomer = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId, associateCustomerDetails);
            logger.WriteLine("Response Associate Customer with blank customerID and blank Status: " + responseAssociateCustomer);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseAssociateCustomer.Item1);
        }

        [Fact]
        public void Test790429_Create_Associate_Customer_ProductEnvironments()
        {
            //Create new Customer to get customerId
            customerId = $"{Guid.NewGuid()}";
            var customerDetails = CustomerFactory.createCustomer(customerId, $"Customer_{StringUtils.RandomString(10)}", true);
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            //Create associate customer and product environment with invalid GUID and valid customerid and active status
            var associateCustomerDetails = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer = EnvironmentAPIs.Associate_Customer_With_Product_Environment("invalidProductEnvironmentId", associateCustomerDetails);
            logger.WriteLine("Response Associate Customer with Invalid GUID: " + responseAssociateCustomer);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseAssociateCustomer.Item1);
            Assert.Equal(expected: $"The value 'invalidProductEnvironmentId' is not valid.", JObject.Parse(responseAssociateCustomer.Item2)["id"][0].ToString());
        }

        [Fact]
        public void Test790433_Create_Associate_Customer_and_Product_Environment_with_No_Exist_GUID_and_Valid_CustomerID_Active_Status()
        {
            //Create new Customer to get customerId
            customerId = $"{Guid.NewGuid()}";
            var customerDetails = CustomerFactory.createCustomer(customerId, $"Customer_{StringUtils.RandomString(10)}", true);
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            //Create associate customer and product environment with no Exist productEnvironmentId
            var associateCustomerDetails = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer = EnvironmentAPIs.Associate_Customer_With_Product_Environment($"{Guid.NewGuid()}", associateCustomerDetails);
            logger.WriteLine("Response Associate Customer with Invalid GUID: " + responseAssociateCustomer);
            Assert.Equal(expected: HttpStatusCode.NotFound, responseAssociateCustomer.Item1);
        }

        [Fact]
        public void Test790434_Create_Associate_Customer_and_Product_Environment_with_Valid_GUID_and_CustomerID_Match_with_Another_But_Active_Status_Not_Match()
        {
            //// Initialze the step to create Associate Customer
            //Test790412_Create_Associate_Customer_And_Product_Environment_With_ValidGUID_And_RequestBody();

            ////Create new Customer2
            //customerId2 = $"{Guid.NewGuid()}";
            //var customerDetails = CustomerFactory.createCustomer(customerId2, $"Customer_{StringUtils.RandomString(10)}", false);
            //Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            //Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            ////Create associate customer and product environment with Valid productEnvironmentId and customerId match with another productEnvironmentId
            //associateCustomerDetails2 = AssociateFactory.createAssociate(customerId, true);
            //Tuple<HttpStatusCode, String> responseAssociateCustomer = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId2, associateCustomerDetails2);
            //logger.WriteLine("Response Associate Customer with Invalid productEnvironmentId and customerId match with another productEnvironmentId: " + responseAssociateCustomer);
            //Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer.Item1);

            //Create new ProductEnvironment
            Create_New_Product();
            Create_New_ProductEnvironment();
            Create_New_Customer();

            //Associate Customer to Product Environment, active = false
            associateCustomerDetails = AssociateFactory.createAssociate(customerId, false);
            Tuple<HttpStatusCode, String> responseAssociateCustomer = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId, associateCustomerDetails);
            logger.WriteLine("Response Associate Customer with Invalid productEnvironmentId and customerId match with another productEnvironmentId: " + responseAssociateCustomer);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer.Item1);
            Assert.Equal(expected: "False", JObject.Parse(responseAssociateCustomer.Item2)["active"].ToString());

            //Associate Customer to Product Environment, active = true
            associateCustomerDetails2 = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer2 = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId, associateCustomerDetails2);
            logger.WriteLine("Response Associate Customer with Invalid productEnvironmentId and customerId match with another productEnvironmentId: " + responseAssociateCustomer2);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer2.Item1);
            Assert.Equal(expected: "True", JObject.Parse(responseAssociateCustomer2.Item2)["active"].ToString());

        }

        [Fact]
        public void Test838272_Delete_Customer_To_Product_Environment_Associate_with_Valid_CustomerID_And_ProductEnvironmentID()
        {
            //Dissassociate Customer And Product Environment
            productEnvironmentId = $"{Guid.NewGuid()}";
            customerId = $"{Guid.NewGuid()}";
            Tuple<HttpStatusCode, String> responseDisassociateCustomer = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment(productEnvironmentId, customerId);
            logger.WriteLine("Response Disassociate Customer: " + responseDisassociateCustomer);
            Assert.Equal(expected: HttpStatusCode.NotFound, responseDisassociateCustomer.Item1);
        }

        [Fact]
        public void Test838273_Delete_Customer_to_ProductEnvironment_Association_with_Valid_customerId_and_productEnvironmentID_have_User_ProductEnvironment()
        {
            //Create new ProductEnvironment
            Create_New_Product();
            Create_New_ProductEnvironment();
            Create_New_Customer();

            //Associate Customer And Product Environment
            associateCustomerDetails = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId, associateCustomerDetails);
            logger.WriteLine("Response Associate Customer: " + responseAssociateCustomer);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer.Item1);

            //Create User with product (1)
            string userName = StringUtils.RandomString(10);
            userWithProductDetails = EntitlementFactory.createEntitlement(productId, customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //Delete a customer to product-environment association with valid customerId and invalid productEnvironmentId
            Tuple<HttpStatusCode, String> responseDisassociateCustomer = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment(productEnvironmentId, customerId);
            logger.WriteLine("Response Disassociate Customer with Invalid productEnvironmentId: " + responseDisassociateCustomer);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseDisassociateCustomer.Item1);
        }

        [Fact]
        public void Test838274_Delete_A_Customer_To_Product_Environment_Association_with_Valid_CustomerID_and_Invalid_ProductEnvironmentID()
        {
            // Initialze the step to create Associate Customer
            Test790412_Create_Associate_Customer_And_Product_Environment_With_ValidGUID_And_RequestBody();

            //Delete a customer to product-environment association with valid customerId and invalid productEnvironmentId
            Tuple<HttpStatusCode, String> responseDisassociateCustomer = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment($"Invalid_{productEnvironmentId}", customerId);
            logger.WriteLine("Response Disassociate Customer with Invalid productEnvironmentId: " + responseDisassociateCustomer);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseDisassociateCustomer.Item1);
        }

        [Fact]
        public void Test838795_Delete_A_Customer_To_Product_Environment_Association_with_No_Exist_CustomerID_and_Valid_ProductEnvironmentID()
        {
            // Initialze the step to create Associate Customer
            Test790412_Create_Associate_Customer_And_Product_Environment_With_ValidGUID_And_RequestBody();

            //Delete a customer to product-environment association with no exist customerId and valid productEnvironmentId
            Tuple<HttpStatusCode, String> responseDisassociateCustomer = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment(productEnvironmentId, $"{Guid.NewGuid()}");
            logger.WriteLine("Response Disassociate Customer with no exist customerId and Valid productEnvironmentId: " + responseDisassociateCustomer);
            Assert.Equal(expected: HttpStatusCode.NotFound, responseDisassociateCustomer.Item1);
        }

        [Fact]
        public void Test838721_Delete_Customer_To_Product_Environment_Associate_with_Valid_CustomerID_And_ProductEnvironmentID_no_User_Customer_ProductEnvironment_and_have_User()
        {
            //Create new ProductEnvironment
            Create_New_Product();
            Create_New_ProductEnvironment();
            Create_New_Customer();

            //Associate Customer And Product Environment
            associateCustomerDetails = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId, associateCustomerDetails);
            logger.WriteLine("Response Associate Customer: " + responseAssociateCustomer);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer.Item1);

            //Create new productLine (2)
            productLineId2 = $"{Guid.NewGuid()}";
            string productLineName = $"ProductLine_{StringUtils.RandomString(10)}";
            var productLineDetails = ProductLineFactory.createProductLine(productLineId2, productLineName);
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

            // Create new product (2)
            productId2 = $"{Guid.NewGuid()}";
            string type = "API";
            var productDetails = ProductFactory.createProductList(productId2, $"Product_{StringUtils.RandomString(10)}", StringUtils.RandomString(5), productIconColor, productLineName, solutionFamilyId, type);
            Tuple<HttpStatusCode, String> responseCreateProduct1 = ProductAPIs.Create_Products(productDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProduct1.Item1);

            // Create new product environment (2)
            productEnvironmentId2 = $"{Guid.NewGuid()}";
            var productEnvironmentDetails2 = EnvironmentFactory.createEnvironment(productEnvironmentId2, productId2, $"Product_Environment_{StringUtils.RandomString(10)}", $"https://testAPI.com/{productEnvironmentId2}", StringUtils.RandomString(5));
            Tuple<HttpStatusCode, String> responseCreateProductEnvironment2 = EnvironmentAPIs.Create_New_Product_Environment(productEnvironmentDetails2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductEnvironment2.Item1);

            //Associate Customer And Product Environment (2)
            associateCustomerDetails2 = AssociateFactory.createAssociate(customerId, true);
            Tuple<HttpStatusCode, String> responseAssociateCustomer2 = EnvironmentAPIs.Associate_Customer_With_Product_Environment(productEnvironmentId2, associateCustomerDetails2);
            logger.WriteLine("Response Associate Customer: " + responseAssociateCustomer2);
            Assert.Equal(expected: HttpStatusCode.OK, responseAssociateCustomer2.Item1);

            //Create User with product (2)
            string userName = StringUtils.RandomString(10);
            userWithProductDetails = EntitlementFactory.createEntitlement(productId2, customerId, $"email_{userName}@gmail.com", userName, true);
            Tuple<HttpStatusCode, String> responseCreateuserWithProduct = EntitlementAPIs.Create_Entitlements_For_The_Product_of_Customer(userWithProductDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateuserWithProduct.Item1);

            //Delete a customer to product-environment association with valid customerId and invalid productEnvironmentId
            Tuple<HttpStatusCode, String> responseDisassociateCustomer = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment(productEnvironmentId, customerId);
            logger.WriteLine("Response Disassociate Customer with Invalid productEnvironmentId: " + responseDisassociateCustomer);
            Assert.Equal(expected: HttpStatusCode.OK, responseDisassociateCustomer.Item1);
        }

        [Fact]
        public void Test838799_Delete_Customer_to_ProductEnvironment_Association_with_blank_customerId_and_Valid_productEnvironmentId()
        {
            //Create new ProductEnvironment
            Create_New_Product();
            Create_New_ProductEnvironment();
            Create_New_Customer();

            //Delete a customer to product-environment association with valid customerId and invalid productEnvironmentId
            Tuple<HttpStatusCode, String> responseDisassociateCustomer = EnvironmentAPIs.Disassociate_Customer_From_Product_Environment(productEnvironmentId, null);
            logger.WriteLine("Response Disassociate Customer with Invalid productEnvironmentId: " + responseDisassociateCustomer);
            Assert.Equal(expected: HttpStatusCode.NotImplemented, responseDisassociateCustomer.Item1);
        }




        //Setup function (Create new Product)
        public void Create_New_Product()
        {
            //// Create new customer to get customerID
            //customerId = $"{Guid.NewGuid()}";
            //var customerDetails = CustomerFactory.createCustomer(customerId, $"Customer_{StringUtils.RandomString(10)}", true, $"{Guid.NewGuid()}", $"{Guid.NewGuid()}", $"ProductEnvironment_{StringUtils.RandomString(6)}", $"https://spheracloud_{StringUtils.RandomString(10)}.net", StringUtils.RandomString(6));
            //Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            //Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);

            // Create new productLine
            productLineId = $"{Guid.NewGuid()}";
            string productLineName = $"ProductLine_{StringUtils.RandomString(10)}";
            var productLineDetails = ProductLineFactory.createProductLine(productLineId, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProductLine = ProductLineAPIs.Create_New_ProductLine(productLineDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductLine.Item1);

            // Create new product
            productId = $"{Guid.NewGuid()}";
            string type = "API";
            var productDetails = ProductFactory.createProductList(productId, $"Product_{StringUtils.RandomString(10)}", StringUtils.RandomString(5), productIconColor, productLineName, solutionFamilyId, type);
            Tuple<HttpStatusCode, String> responseCreateProduct1 = ProductAPIs.Create_Products(productDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProduct1.Item1);
        }

        //Setup function (Create new product environment)
        public void Create_New_ProductEnvironment()
        {
            productEnvironmentId = $"{Guid.NewGuid()}";
            productEnvironmentName = $"Product_Environment_{StringUtils.RandomString(10)}";
            urlProductEnvironment = $"https://testAPI.com/{productEnvironmentId}";
            productEnvironmentCode = StringUtils.RandomString(5);
            var productEnvironmentDetails = EnvironmentFactory.createEnvironment(productEnvironmentId, productId, productEnvironmentName, urlProductEnvironment, productEnvironmentCode);
            Tuple<HttpStatusCode, String> responseCreateProductEnvironment = EnvironmentAPIs.Create_New_Product_Environment(productEnvironmentDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductEnvironment.Item1);
            //logger.WriteLine("body ProductEnvironment: " + JsonConvert.SerializeObject(productEnvironmentDetails).ToString());
        }

        public void Create_New_Customer()
        {
            // Create new customer to get customerID
            customerId = $"{Guid.NewGuid()}";
            string customerName = $"Customer_{StringUtils.RandomString(10)}";
            var customerDetails = CustomerFactory.createCustomer(customerId, customerName, true);
            Tuple<HttpStatusCode, String> responseCreateCustomer = CustomerAPIs.Create_Customers_Details(customerDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateCustomer.Item1);
        }
    }
}