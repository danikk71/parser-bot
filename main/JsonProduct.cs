using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace main
{
    class JSONProduct
    {
        private static string GetDirectoryPath()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "DataJSON");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return folderPath;
        }
        public static async Task Serialize(string key,List<Product> products)
        {
            string fileName = Path.Combine(GetDirectoryPath(), $"{key}-{DateTime.UtcNow.Year}.{DateTime.UtcNow.Month}.{DateTime.UtcNow.Day}-{DateTime.UtcNow.Hour}.json");
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(products,options);
            await File.WriteAllTextAsync(fileName, json);

            Console.WriteLine("\nДані збережено!\n");
        }
        //public static async Task<List<Product>> Deserialize()
        //{
        //    if (!File.Exists(path)) return new List<Product>();
        //    string json = await File.ReadAllTextAsync(path);

        //    var products = JsonSerializer.Deserialize<List<Product>>(json);
        //    Console.WriteLine("\nДані повернено!\n");
        //    return products ?? new List<Product>();
        //}
    }
}
