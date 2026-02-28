using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using main.Helpers;
using main.Interfaces;
using main.Models;

namespace main.Services.Storage
{
    class JsonService<T> : IExporter<T> , IImporter<T>
    {
        readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public async Task ExportAsync(T products)
        {
            string dataFolder = PathHelper.GetDataFolderPath();
            Directory.CreateDirectory(dataFolder);
            string archiveFolder = Path.Combine(dataFolder, "Archive");
            Directory.CreateDirectory(archiveFolder);

            string actualPathName = Path.Combine(dataFolder, "latest.json");
            string archivePathName = Path.Combine(archiveFolder, $"{DateTime.Now:yyyy-MM-dd}.json");

            await using (FileStream fileStream = File.Create(actualPathName))
            {
                await JsonSerializer.SerializeAsync(fileStream, products, _options);
            }
            Console.WriteLine("\nДанi збережено у актуальнi!\n");

            File.Copy(actualPathName, archivePathName, overwrite: true);
            Console.WriteLine("\nДанi збережено у архiв!\n");
        }

        public T Import()
        {
            throw new NotImplementedException();
        }
    }
}
