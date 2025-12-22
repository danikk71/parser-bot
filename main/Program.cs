using System.Collections.Concurrent;
using main;
using main.Models;

class Program {
    static async Task Main()
    {
        ConcurrentDictionary<string, List<Product>> products = new ConcurrentDictionary<string, List<Product>>();
        await Parser.CreateTasks("https://telemart.ua/ua/city-1482/",products);
        foreach (var productKey in products.Values)
        {
            //ListSorter.SortByPriceAscending(productKey);
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