using System.Collections.Concurrent;
using HtmlAgilityPack;
using main.Interfaces;
using main.Models;
using main.Services.Fetcher;
using main.Services.Mappers;
using main.Services.Scrapers;
using main.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


var services = new ServiceCollection();

var configurations = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

services.AddHttpClient<IWebFetcher, HttpFetcher>();
services.AddSingleton<IMapper<HtmlNode, Product>, TelemartMapper>();
services.AddSingleton<IScraperService, TelemartScraper>();
services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(configurations.GetConnectionString("DefaultConnection"));
});
services.AddJsonService();

var serviceProvider = services.BuildServiceProvider();

var jsonExportService = serviceProvider.GetRequiredService<IExporter<List<Product>>>();
var scraper = serviceProvider.GetRequiredService<IScraperService>();

Console.WriteLine("Початок програми: ");
await scraper.RunScraperAsync();

var products = scraper.GetProductsList();
await jsonExportService.ExportAsync(products);
