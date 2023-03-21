using System.Collections.Generic;

namespace Regression_UAT_Environment
{
    public class ProductList
    {
        public List<Product> products { get; set; }
    }

    public class Product
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? code { get; set; }
        public string? iconColor { get; set; }
        public string? productLine { get; set; }
        public string? solutionFamilyId { get; set; }
        public string? type { get; set; }

    }


    public static class ProductFactory
    {
        public static ProductList createProductList(string id, string name, string code, string iconColor, string productLine, string solutionFamilyId, string type)
        {
            if (id == null)
            {
                return new ProductList
                {
                    products = new List<Product>()
                    {
                        new Product()
                        {
                            name = name,
                            code = code,
                            iconColor = iconColor,
                            productLine = productLine,
                            solutionFamilyId = solutionFamilyId,
                            type = type
                        }
                    }
                };
            }
            else
            {
                return new ProductList
                {
                    products = new List<Product>()
                    {
                        new Product()
                        {
                            id = id,
                            name = name,
                            code = code,
                            iconColor = iconColor,
                            productLine = productLine,
                            solutionFamilyId = solutionFamilyId,
                            type = type
                        }
                    }
                };
            }
        }


        public static ProductList createProductList(List<string> listProductId, List<string> listProductName, List<string> listProductCode, List<string> listProductIconColor, List<string> listProductLineName, List<string> solutionFamilyID, List<string> type)
        {
            List<Product> products = new List<Product>();
            if (listProductId.Count == 0)
            {
                for (int i = 0; i < listProductName.Count; i++)
                {
                    var product = new Product()
                    {
                        name = listProductName[i],
                        code = listProductCode[i],
                        iconColor = listProductIconColor[i],
                        productLine = listProductLineName[i],
                        solutionFamilyId = solutionFamilyID[i],
                        type = type[i]
                    };
                    products.Add(product);
                }
            }
            else
            {
                for (int i = 0; i < listProductId.Count; i++)
                {
                    var product = new Product()
                    {
                        id = listProductId[i],
                        name = listProductName[i],
                        code = listProductCode[i],
                        iconColor = listProductIconColor[i],
                        productLine = listProductLineName[i],
                        solutionFamilyId = solutionFamilyID[i],
                        type = type[i]
                    };
                    products.Add(product);
                }
            }

            return new ProductList
            {
                products = products
            };
        }

        public static Product createProduct(string id, string name, string code, string iconColor, string productLine)
        {
            if (id == null)
            {
                return new Product()
                {
                    name = name,
                    code = code,
                    iconColor = iconColor,
                    productLine = productLine
                };
            }
            else
            {
                return new Product()
                {
                    id = id,
                    name = name,
                    code = code,
                    iconColor = iconColor,
                    productLine = productLine
                };
            }

        }
    }
}
