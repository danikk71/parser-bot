using System;
using System.Text.Json;
using HtmlAgilityPack;
using main;

class Program {
    static async Task Main()
    {
        List<Product> products = await Parser.GetProducts();
        foreach (var product in products)
        {
            Console.WriteLine($"Name: {product.Name}, Price: {product.Price} , Type: {product.Type}");
        }
        await JSONProduct.Serialize(products);
        List<Product> newProducts = await JSONProduct.Deserialize();
        foreach (var product in newProducts)
        {
            Console.WriteLine($"Name: {product.Name}, Price: {product.Price} , Type: {product.Type}");
        }
    }
}