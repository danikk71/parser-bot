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
            string folderPath = Path.Combine(projectRoot.FullName,"Data","latest.json");
            return folderPath;
        }
        public static async Task Serialize(List<Product> products)
        {
            string fileName = GetDirectoryPath() ?? string.Empty;
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(products,options);
            await File.WriteAllTextAsync(fileName, json);

            Console.WriteLine("\nДані збережено!\n");
        }

    }
}
