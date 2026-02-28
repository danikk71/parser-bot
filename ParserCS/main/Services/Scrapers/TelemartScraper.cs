using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using main.Interfaces;
using main.Models;
using main.Services.Storage;

namespace main.Services.Scrapers
{
    public class TelemartScraper : IScraperService
    {
        private const string url = "https://telemart.ua/ua/city-1482";
        private readonly Dictionary<ProductType, string> _urls = new()
        {
            { ProductType.RAM,$"{url}/ram/" },
            { ProductType.CPU,$"{url}/processor/" },
            { ProductType.Motherboard,$"{url}/motherboard/" },
            { ProductType.GPU,$"{url}/videocard/" },
            { ProductType.SSD,$"{url}/ssd/" },
            { ProductType.HDD,$"{url}/hard-drive/" }
        };

        private readonly ConcurrentDictionary<ProductType, List<Product>> _products = new();
        private readonly SemaphoreSlim _semaphore = new(5);
        private readonly IWebFetcher _fetcher;
        private readonly IMapper<HtmlNode,Product> _mapper;
        
        public TelemartScraper(IWebFetcher fetcher, IMapper<HtmlNode, Product> mapper)
        {
            _fetcher = fetcher;
            _mapper = mapper;
        }

        public async Task RunScraperAsync()
        {
            var parsingTasks = new List<Task>();

            foreach(var item in _urls)
                parsingTasks.Add(GetCategoryAsync(item.Key, item.Value));

            await Task.WhenAll(parsingTasks);
        }

        public List<Product> GetProductsList()
        {
            return _products.Values.SelectMany(x => x).ToList() ;
        }

        private async Task GetCategoryAsync(ProductType type, string url)
        {
            await _semaphore.WaitAsync();
            try
            {
                List<Product> categoryProducts = new();
                int pageCount = 1;
                while (true)
                {
                    Console.WriteLine($"Waiting for response at page {pageCount} - {type}");
                    string html = await _fetcher.FetchHTMLAsync(url + $"?page={pageCount}");
                    if (string.IsNullOrEmpty(html))
                    {
                        Console.WriteLine($"End of parse (end of pages) - {type}");
                        break;
                    }

                    var productNodes = ExtractProductNodes(html);
                    if (productNodes == null || productNodes.Count == 0)
                    {
                        Console.WriteLine($"End of parse (no items found) - {type}");
                        break;
                    }

                    foreach(var productNode in productNodes)
                    {
                        var product = _mapper.Map(productNode);
                        if (product != null)
                            categoryProducts.Add(product);
                    }
                    pageCount++;
                    await Task.Delay(1000);
                }

                _products.TryAdd(type, categoryProducts);
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
