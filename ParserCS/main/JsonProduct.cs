using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using main.Models;

namespace main
{
    class JSONProduct
    {
        private static string? GetDirectoryPath()
        {
            var baseDir = AppContext.BaseDirectory;
            DirectoryInfo? projectRoot = Directory.GetParent(baseDir);
            while (projectRoot != null && projectRoot.Name != "parser")
            {
                projectRoot = projectRoot.Parent;
            }
            if (projectRoot == null)
            {
                Console.WriteLine("Не знайшов кореневу папку 'parser'!");
                return null;
            }
            string folderPath = Path.Combine(projectRoot.FullName, $"Data/{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}/{DateTime.Now.Hour}-Hour");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return folderPath;
        }
        public static async Task Serialize(string key,List<Product> products)
        {
            string directoryPath = GetDirectoryPath() ?? string.Empty;
            string fileName = Path.Combine(directoryPath, $"{key}.json");
            
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
