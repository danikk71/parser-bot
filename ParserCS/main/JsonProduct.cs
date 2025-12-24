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
            return projectRoot.FullName;
        }
        public static async Task Serialize(List<Product> products)
        {
            string? directoryName = GetDirectoryPath();
            if (string.IsNullOrEmpty(directoryName)) return;

            string dataFolder = Path.Combine(directoryName, "Data");
            string archiveFolder = Path.Combine(dataFolder, "Archive");
            if (!Directory.Exists(archiveFolder))
            {
                Directory.CreateDirectory(archiveFolder);
            }
            string actualPathName = Path.Combine(dataFolder, "latest.json");
            string archivePathName = Path.Combine(archiveFolder, $"{DateTime.Now:yyyy-MM-dd}.json");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(products,options);

            await File.WriteAllTextAsync(actualPathName, json);
            Console.WriteLine("\nДані збережено у актуальні!\n");
            await File.WriteAllTextAsync(archivePathName, json);
            Console.WriteLine("\nДані збережено у архів!\n");
        }

    }
}
