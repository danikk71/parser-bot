using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using main.Interfaces;
using main.Models;

namespace main.Services
{
    public class TelemartScraper : IScraperService
    {
        private const string url = "https://telemart.ua/ua/city-1482/";
        private readonly Dictionary<ProductType, string> _urls = new()
        {
            { ProductType.RAM,$"{url}/ram/" },
            { ProductType.CPU,$"{url}/processor/" },
            { ProductType.Motherboard,$"{url}/motherboard/" },
            { ProductType.GPU,$"{url}/videocard/" },
            { ProductType.SSD,$"{url}/ssd/" },
            { ProductType.HDD,$"{url}/hard-drive/" }
        };

        private readonly SemaphoreSlim _semaphore = new(5);
        private readonly HttpFetcher _fetcher;
        
        public TelemartScraper(HttpFetcher fetcher)
        {
            _fetcher = fetcher;
        }

        public Task RunScraperAsync()
        {
            var parsingTasks = new List<Task>();

            foreach(var item in _urls)
            {
                parsingTasks.Add(GetCategoryAsync(item.Key, item.Value));
            }
        }

        private async Task GetCategoryAsync(ProductType type, string url)
        {
            await _semaphore.WaitAsync();
            try
            {
                int pageCount = 1;
                while (true)
                {
                    Console.WriteLine($"Waiting for response at page {pageCount} - {type}");
                    string html = await _fetcher.FetchHTMLAsync(url + $"?page={pageCount}");

                    var productNodes = ExtractProductNodes(html);
                    if (productNodes == null || productNodes.Count == 0)
                    {
                        Console.WriteLine($"End of parse - {type}");
                        break;
                    }
                    foreach(var productNode in productNodes)
                    {
                        //mapper
                    }
                    pageCount++;
                    await Task.Delay(1000);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private HtmlNodeCollection? ExtractProductNodes(string html) 
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            return htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'product-item col-lg-3')]");
        }
    }
}
