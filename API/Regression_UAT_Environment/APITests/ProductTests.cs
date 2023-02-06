using Xunit;
using System.Net;
using Xunit.Abstractions;
using System;
using Regression_UAT_Environment.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Regression_UAT_Environment.Tests
{
    public class ProductTests : IDisposable
    {
        private readonly ITestOutputHelper logger;
        private string productId;
        private string productName;
        private string productCode;
        private string productLineName;
        private string productLineId;
        private string productIconColor = String.Format("#{0:X6}", new Random().Next(0x1000000));
        private ProductList productDetails;
        private List<string> listProductId = new List<string>();
        private List<string> listProductName = new List<string>();
        private List<string> listProductCode = new List<string>();
        private List<string> listProductIconColor = new List<string>();
        private List<string> listProductLineName = new List<string>();

        public ProductTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        //Teardown
        public void Dispose()
        {
            //Cleanup Product
            if (productId != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteProduct2 = ProductAPIs.Delete_Product_From_Entitlements_By_Id(productId);
            }
            //cleanup multiple Product
            if (listProductId.Count > 0)
            {
                foreach (string productIdClean in listProductId)
                {
                    Tuple<HttpStatusCode, String> responseCleanProduct = ProductAPIs.Delete_Product_From_Entitlements_By_Id(productIdClean);
                }
            }
            //Cleanup ProductLine
            if (productLineId != null)
            {
                Tuple<HttpStatusCode, String> responseDeleteProductLine = ProductLineAPIs.Delete_ProductLine_By_Id(productLineId);
            }
            logger.WriteLine("Successfully Cleanup the Product Tests");
        }


        [Fact]
        public void Test777409_Get_Available_Product()
        {
            Tuple<HttpStatusCode, String> responseGetAllProductLines = ProductLineAPIs.Get_All_Available_ProductLines();
            logger.WriteLine("Response Get All Avaiable ProductLines: " + responseGetAllProductLines);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetAllProductLines.Item1);
        }

        [Fact]
        public void Test782501_Create_product_with_all_valid_value()
        {
            //Create new productLine
            Create_New_ProductLine();

            //Create new Product
            productId = $"{Guid.NewGuid()}";
            productName = $"Product_{StringUtils.RandomString(10)}";
            productCode = StringUtils.RandomString(5);
            productDetails = ProductFactory.createProductList(productId, productName, productCode, productIconColor, productLineName);
            logger.WriteLine("body" + JsonConvert.SerializeObject(productDetails).ToString());
            Tuple<HttpStatusCode, String> responseCreateProduct = ProductAPIs.Create_Products(productDetails);
            logger.WriteLine("Response Create Product: " + responseCreateProduct);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProduct.Item1);
            Assert.Equal(expected: productId, JArray.Parse(responseCreateProduct.Item2)[0]["id"].ToString());
            Assert.Equal(expected: productName, JArray.Parse(responseCreateProduct.Item2)[0]["name"].ToString());
            Assert.Equal(expected: productCode, JArray.Parse(responseCreateProduct.Item2)[0]["code"].ToString());
            Assert.Equal(expected: productIconColor, JArray.Parse(responseCreateProduct.Item2)[0]["iconColor"].ToString());
            Assert.Equal(expected: productLineName, JArray.Parse(responseCreateProduct.Item2)[0]["productLine"].ToString());
        }

        [Fact]
        public void Test782502_Create_Multi_Product_With_All_Valid_Value()
        {
            //Create new productLine
            Create_New_ProductLine();

            //Create information of 5 Products
            for (int i = 0; i < 5; i++)
            {
                listProductId.Add($"{Guid.NewGuid()}");
                listProductName.Add($"Product_{StringUtils.RandomString(10)}");
                listProductCode.Add(StringUtils.RandomString(6));
                listProductIconColor.Add(productIconColor);
                listProductLineName.Add(productLineName);
            }
            //Create Multiple Product
            var multipleProductDetails = ProductFactory.createProductList(listProductId, listProductName, listProductCode, listProductIconColor, listProductLineName);
            Tuple<HttpStatusCode, String> responseCreateMultipleProduct = ProductAPIs.Create_Products(multipleProductDetails);
            logger.WriteLine("Response Create Multiple Product: " + responseCreateMultipleProduct);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateMultipleProduct.Item1);
        }

        [Fact]
        public void Test782503_Post_Create_One_Product_Without_GUID()
        {
            //Create new ProductLine
            Create_New_ProductLine();

            //Create new Product
            productId = null;
            productName = $"Product_{StringUtils.RandomString(10)}";
            productCode = StringUtils.RandomString(5);
            productDetails = ProductFactory.createProductList(productId, productName, productCode, productIconColor, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProduct = ProductAPIs.Create_Products(productDetails);
            logger.WriteLine("Response Create Product: " + responseCreateProduct);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProduct.Item1);
        }
        [Fact]
        public void Test782594_Post_Create_Multi_Product_Without_Product_Name_Or_Code_Or_Color_Or_ProductLine()
        {
            //Create new productLine
            Create_New_ProductLine();

            //Create information of 5 Products
            List<string> listProductName = new List<string> { null, null, StringUtils.RandomString(6), productIconColor, productLineName };
            List<string> listProductCode = new List<string> { null, $"Product_{StringUtils.RandomString(10)}", null, productIconColor, productLineName };
            List<string> listProductIconColor = new List<string> { null, $"Product_{StringUtils.RandomString(10)}", StringUtils.RandomString(6), null, productLineName };
            List<string> listProductLineName = new List<string> { null, $"Product_{StringUtils.RandomString(10)}", StringUtils.RandomString(6), productIconColor, null };

            //Create Multiple Product
            var multipleProductDetails = ProductFactory.createProductList(listProductId, listProductName, listProductCode, listProductIconColor, listProductLineName);
            Tuple<HttpStatusCode, String> responseCreateMultipleProduct = ProductAPIs.Create_Products(multipleProductDetails);
            logger.WriteLine("Response Create Multiple Product without productName/Code/Color/roductLine:" + responseCreateMultipleProduct);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateMultipleProduct.Item1);
        }

        [Fact]
        public void Test782601_Post_Create_Multi_Product_with_Invalid_ProductLine()
        {
            productLineName = $"Invalid ProductLine Name {StringUtils.RandomString(10)}";

            //Create information of 5 Products
            List<string> listProductName = new List<string>();
            List<string> listProductCode = new List<string>();
            List<string> listProductIconColor = new List<string>();
            List<string> listProductLineName = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                listProductId.Add($"{Guid.NewGuid()}");
                listProductName.Add($"Product_{StringUtils.RandomString(10)}");
                listProductCode.Add(StringUtils.RandomString(6));
                listProductIconColor.Add(productIconColor);
                listProductLineName.Add(productLineName);
            }
            //Create Multiple Product
            var multipleProductDetails = ProductFactory.createProductList(listProductId, listProductName, listProductCode, listProductIconColor, listProductLineName);
            Tuple<HttpStatusCode, String> responseCreateMultipleProduct = ProductAPIs.Create_Products(multipleProductDetails);
            logger.WriteLine("Response Create Multiple Product with Invalid ProductLine: " + responseCreateMultipleProduct);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateMultipleProduct.Item1);
        }

        [Fact]
        public void Test782666_Post_Create_Product_with_Name_Match_with_another_Product()
        {
            //Create new ProductLine
            Create_New_ProductLine();

            //Create new Product (1)
            productId = $"{Guid.NewGuid()}";
            productName = $"Product_{StringUtils.RandomString(10)}";
            productCode = StringUtils.RandomString(5);
            productDetails = ProductFactory.createProductList(productId, productName, productCode, productIconColor, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProduct = ProductAPIs.Create_Products(productDetails);
            logger.WriteLine("Response Create Product: " + responseCreateProduct.Item2);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProduct.Item1);

            //Create new Product (2) with Product Name same with Product Name of Product (1)
            string productId2 = $"{Guid.NewGuid()}";
            string productCode2 = StringUtils.RandomString(5);
            var productDetails2 = ProductFactory.createProductList(productId2, productName, productCode2, productIconColor, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProduct2 = ProductAPIs.Create_Products(productDetails2);
            logger.WriteLine("Response Create Product with Name match with another Product: " + responseCreateProduct2);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseCreateProduct2.Item1);
        }

        [Fact]        
        public void Test782681_Update_Product_with_Valid_GUID_and_RequestBody()
        {
            // Initialze the step to get the valid productId and productLineId ID from database
            Test782501_Create_product_with_all_valid_value();

            //Update Product with valid ID and requestBody
            productDetails.products[0].name = $"NewProduct_{StringUtils.RandomString(10)}";
            string productNameUpdate = productDetails.products[0].name;
            Tuple<HttpStatusCode, String> responseUpdateProduct = ProductAPIs.Update_Product_By_Id(productId, productDetails.products[0]);
            //logger.WriteLine("Response Update Product: " + responseUpdateProduct);
            Assert.Equal(expected: HttpStatusCode.OK, responseUpdateProduct.Item1);
            Assert.Equal(expected: productNameUpdate, JObject.Parse(responseUpdateProduct.Item2)["name"].ToString());
        }

        [Fact]
        public void Test782684_Update_Product_with_Invalid_GUID_and_Valid_RequestBody()
        {
            //Update Product with invalid ID and valid requestBody
            productId = $"Invalid_{Guid.NewGuid()}";
            productName = $"Product_{StringUtils.RandomString(10)}";
            productCode = StringUtils.RandomString(5);
            productDetails = ProductFactory.createProductList(productId, productName, productCode, productIconColor, productLineName);
            Tuple<HttpStatusCode, String> responseUpdateProduct = ProductAPIs.Update_Product_By_Id(productId, productDetails.products[0]);
            logger.WriteLine("Response Update Product with Invalid ID and valid RequestBody: " + responseUpdateProduct);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateProduct.Item1);
        }

        [Fact]
        public void Test782704_Update_Product_with_Valid_GUID_and_Blank_RequestBody()
        {
            // Initialze the step to get the valid productId and productLineId ID from database
            Test782501_Create_product_with_all_valid_value();

            //Update Product with valid ID and blank requestBody
            Tuple<HttpStatusCode, String> responseUpdateProduct = ProductAPIs.Update_Product_By_Id(productId, null);
            logger.WriteLine("Response Update Product with valid ID and blank RequestBody: " + responseUpdateProduct);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateProduct.Item1);
        }

        [Fact]
        public void Test782712_Update_Product_with_Name_and_Product_Line_Match_with_Another_Product()
        {
            // Initialze the step to get the valid productId and productLineId ID from database
            Test782502_Create_Multi_Product_With_All_Valid_Value();

            //Update Product with Name and ProductLine match with another Product
            productDetails = ProductFactory.createProductList(listProductId[0], listProductName[1], listProductCode[0], listProductIconColor[0], listProductLineName[1]);
            Tuple<HttpStatusCode, String> responseUpdateProduct = ProductAPIs.Update_Product_By_Id(listProductId[0], productDetails.products[0] );
            logger.WriteLine("Response Update Product with Name and ProductLine match with another Product: " + responseUpdateProduct);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseUpdateProduct.Item1);
        }

        [Fact]
        public void Test782713_Delete_Product_with_Valid_GUID_This_Product_Not_Associated_with_Custome_or_Product_or_Entitlements()
        {
            // Initialze the step to get the valid productId and productLineId ID from database
            Test782501_Create_product_with_all_valid_value();

            //Delete Product
            Tuple<HttpStatusCode, String> responseDeleteProduct = ProductAPIs.Delete_Product_From_Entitlements_By_Id(productId);
            logger.WriteLine("Response Status Code (Delete Product): " + responseDeleteProduct.Item1);
            Assert.Equal(expected: HttpStatusCode.OK, responseDeleteProduct.Item1);

            //Get product with valid productId to verify successfully delete Product
            Tuple<HttpStatusCode, String> responseGetProduct = ProductAPIs.Get_Product_By_Id(productId);
            logger.WriteLine("Response Get Product: " + responseGetProduct);
            Assert.Equal(expected: HttpStatusCode.NotFound, responseGetProduct.Item1);
        }

        [Fact]
        public void Test782714_Delete_Product_with_Invalid_GUID()
        {
            //Delete Product
            productId = $"Invalid_{Guid.NewGuid()}";
            Tuple<HttpStatusCode, String> responseDeleteProduct = ProductAPIs.Delete_Product_From_Entitlements_By_Id(productId);
            logger.WriteLine("Response Delete Product with Invalid ID: " + responseDeleteProduct);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseDeleteProduct.Item1);
        }

        [Fact]
        public void Test782717_Get_Product_With_Valid_GUID()
        {
            // Initialze the step to get the valid productId and productLineId ID from database
            Test782501_Create_product_with_all_valid_value();
            //Get product with valid productId
            Tuple<HttpStatusCode, String> responseGetProduct = ProductAPIs.Get_Product_By_Id(productId);
            logger.WriteLine("Response Get Product: " + responseGetProduct);
            Assert.Equal(expected: HttpStatusCode.OK, responseGetProduct.Item1);
            Assert.Equal(expected: productId, JObject.Parse(responseGetProduct.Item2)["id"].ToString());
            Assert.Equal(expected: productName, JObject.Parse(responseGetProduct.Item2)["name"].ToString());
            Assert.Equal(expected: productCode, JObject.Parse(responseGetProduct.Item2)["code"].ToString());
            Assert.Equal(expected: productIconColor, JObject.Parse(responseGetProduct.Item2)["iconColor"].ToString());
            Assert.Equal(expected: productLineName, JObject.Parse(responseGetProduct.Item2)["productLine"].ToString());
        }

        [Fact]
        public void Test782718_Get_Product_with_Invalid_GUID()
        {
            //Get product with invalid productId
            productId = $"Invalid_{Guid.NewGuid()}";
            Tuple<HttpStatusCode, String> responseGetProduct = ProductAPIs.Get_Product_By_Id(productId);
            logger.WriteLine("Response Get Product with Invalid ID: " + responseGetProduct);
            Assert.Equal(expected: HttpStatusCode.BadRequest, responseGetProduct.Item1);
        }


        //Set up function (Create new ProductLine)
        public void Create_New_ProductLine()
        {
            //Create new productLine
            productLineId = $"{Guid.NewGuid()}";
            productLineName = $"ProductLine_{StringUtils.RandomString(10)}";
            var productLineDetails = ProductLineFactory.createProductLine(productLineId, productLineName);
            Tuple<HttpStatusCode, String> responseCreateProductLine = ProductLineAPIs.Create_New_ProductLine(productLineDetails);
            Assert.Equal(expected: HttpStatusCode.OK, responseCreateProductLine.Item1);
        } 
    }
}