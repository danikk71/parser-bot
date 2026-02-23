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
    class JSONProduct<T> : IExporter<T>
    {
        readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public async Task ExportAsync(IEnumerable<T> products)
        {
            string? directoryName = Directory.GetCurrentDirectory();
            if (string.IsNullOrEmpty(directoryName))
                throw new Exception("Шляху до поточної папки не існує!");

            string dataFolder = Path.Combine(directoryName, "Data");
            Directory.CreateDirectory(dataFolder);
            string archiveFolder = Path.Combine(dataFolder, "Archive");
            Directory.CreateDirectory(archiveFolder);

            string actualPathName = Path.Combine(dataFolder, "latest.json");
            string archivePathName = Path.Combine(archiveFolder, $"{DateTime.Now:yyyy-MM-dd}.json");

            await using (FileStream fileStream = File.Create(actualPathName))
            {
                await JsonSerializer.SerializeAsync(fileStream, products, _options);
            }
            Console.WriteLine("\nДані збережено у актуальні!\n");

            File.Copy(actualPathName, archivePathName, overwrite: true);
            Console.WriteLine("\nДані збережено у архів!\n");
        }
    }
}
