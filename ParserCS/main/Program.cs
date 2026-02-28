using System.Collections.Concurrent;
using HtmlAgilityPack;
using main.Interfaces;
using main.Models;
using main.Services;
using main.Services.Storage;
using Microsoft.Extensions.DependencyInjection;


var services = new ServiceCollection();

services.AddHttpClient<IWebFetcher, HttpFetcher>();
services.AddSingleton<IMapper<HtmlNode, Product>, TelemartMapper>();
services.AddTransient<IScraperService, TelemartScraper>();
services.AddJsonService();

var serviceProvider = services.BuildServiceProvider();

var jsonExportService = serviceProvider.GetRequiredService<IExporter<List<Product>>>();
var scraper = serviceProvider.GetRequiredService<IScraperService>();

Console.WriteLine("Початок програми: ");
await scraper.RunScraperAsync();

var products = scraper.GetProductsList();
await jsonExportService.ExportAsync(products);
