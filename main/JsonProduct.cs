using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace main
{
    class JSONProduct
    {
        private static readonly string path = $"products-{DateTime.UtcNow.Hour:D2}:{DateTime.UtcNow.Minute:D2}.json";
        public static async Task Serialize(List<Product> products)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(products,options);

            await File.WriteAllTextAsync(path, json);

            Console.WriteLine("\nДані збережено!\n");
        }
        public static async Task<List<Product>> Deserialize()
        {
            if (!File.Exists(path)) return new List<Product>();
            string json = await File.ReadAllTextAsync(path);

            var products = JsonSerializer.Deserialize<List<Product>>(json);
            Console.WriteLine("\nДані повернено!\n");
            return products ?? new List<Product>();
        }
    }
}
