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
            foreach(var product in productKey)
            {
                Console.WriteLine($"Name: {product.Name}, Price: {product.Price}");
            }
        }
        List<Product> allProducts = products.Values.SelectMany(x => x).ToList();
        await JSONProduct.Serialize(allProducts);
    }
}