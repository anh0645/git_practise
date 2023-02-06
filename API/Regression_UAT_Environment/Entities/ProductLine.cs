namespace Regression_UAT_Environment
{
    public class ProductLine
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }


    public static class ProductLineFactory
    {
        public static ProductLine createProductLine(string id, string name)
        {
            if (id == null)
            {
                return new ProductLine
                {
                    name = name
                };
            }
            else
            {
                return new ProductLine
                {
                    id = id,
                    name = name
                };
            }
            
        }
    }
}
