using Xunit;
using System.Net;
using Xunit.Abstractions;
using System;
using Newtonsoft.Json.Linq;
using Regression_UAT_Environment.Utils;

namespace Regression_UAT_Environment.Tests
{
    public class ProductLineTests : IDisposable
    {
        private readonly ITestOutputHelper logger;
        private string productLineId;

        public ProductLineTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        //Teardown
        public void Dispose()
        {
            //Cleanup ProductLine
            if (productLineId != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteProductLine = ProductLineAPIs.Delete_ProductLine_By_Id(productLineId);
            }
            logger.WriteLine("Successfully Cleanup the ProductLine Tests");
        }


        [Fact]
        public void Test785444_Get_ProductLine_With_Invalid_GUID()
        {
            string invalidProductLineId = $"Invalid_GUID_{Guid.NewGuid()}";
            Tuple<HttpStatusCode, String> responseGetProductLine = ProductLineAPIs.Get_ProductLine_By_Id(invalidProductLineId);
            logger.WriteLine("Response Get productLine with Invalid GUID: " + responseGetProductLine);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseGetProductLine.Item1);
            Assert.Equal(expected: $"The value '{invalidProductLineId}' is not valid.", JObject.Parse(responseGetProductLine.Item2)["id"][0].ToString());
        }

        [Fact]
        public void Test785469_Update_ProductLine_With_Invalid_GUID_Blank_Name()
        {
            string invalidProductLineId = $"Invalid_GUID_{Guid.NewGuid()}";
            var productLineDetails = ProductLineFactory.createProductLine(null, "");
            Tuple<HttpStatusCode, String> responseUpdateProductLine = ProductLineAPIs.Update_ProductLines_By_Id(invalidProductLineId, productLineDetails);
            logger.WriteLine("Response Update productLine with Invalid GUID and Blank Name: " + responseUpdateProductLine);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateProductLine.Item1);
            Assert.Equal(expected: $"The value '{invalidProductLineId}' is not valid.", JObject.Parse(responseUpdateProductLine.Item2)["id"][0].ToString());
        }

        [Fact]
        public void Test786471_Create_new_product_line_with_blank_id_and_valid_name()
        {
            //Create new productLine with Blank Id
            string productLineName = $"ProductLine_{StringUtils.RandomString(10)}";
            var productLineDetails = ProductLineFactory.createProductLine(null, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProductLine = ProductLineAPIs.Create_New_ProductLine(productLineDetails);
            logger.WriteLine("Response Create New productLine with Blank Id: " + responseCreateProductLine);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductLine.Item1);

            //Get list Productline
            Tuple<HttpStatusCode, String> responseGetProductLine = ProductLineAPIs.Get_All_Available_ProductLines();
            Assert.Equal(expected: HttpStatusCode.OK, responseGetProductLine.Item1);
            JArray productLineArray = JArray.Parse(responseGetProductLine.Item2);

            //Check the product is create or not based productLineName
            bool checkProductLineNameExist = false;
            for (int i=0; i < productLineArray.Count; i++)
            {
                if(productLineArray[i]["name"].ToString()== productLineName)
                {
                    checkProductLineNameExist = true;
                    //Get productLineId to cleanup it
                    productLineId = productLineArray[i]["id"].ToString();
                    break;
                }
            }
            Assert.True(checkProductLineNameExist);
        }

        [Fact]
        public void Test786472_Post_Create_Product_Line_Invalid_Id_and_Valid_Name()
        {
            //Create new productLine with Invalid Id and Valid Name
            string invalidProductLineId = $"Invalid_GUID_{Guid.NewGuid()}";
            string productLineName = $"ProductLine_{StringUtils.RandomString(10)}";
            var productLineDetails = ProductLineFactory.createProductLine(invalidProductLineId, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProductLine = ProductLineAPIs.Create_New_ProductLine(productLineDetails);
            logger.WriteLine("Response Create New productLine with Blank Id: " + responseCreateProductLine);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateProductLine.Item1);
        }
    }
}