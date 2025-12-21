using main;

class Program {
    static async Task Main()
    {
        Dictionary<string, List<Product>> products = await Parser.GetProductsFromPage("https://telemart.ua/ua/city-1482/ram/");
        foreach (var productKey in products.Values)
        {
            ListSorter.SortByPriceAscending(productKey);
            foreach(var product in productKey)
            {
                Console.WriteLine($"Name: {product.Name}, Price: {product.Price}");
            }
        }
        foreach(var product in products)
        {
            await JSONProduct.Serialize(product.Key,product.Value);
        }
        //List<Product> newProducts = await JSONProduct.Deserialize();
        //foreach (var product in newProducts)
        //{
        //    Console.WriteLine($"Name: {product.Name}, Price: {product.Price}");
        //}
    }
}