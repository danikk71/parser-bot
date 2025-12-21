using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace main
{
    class Parser
    {
        static SemaphoreSlim semaphore = new SemaphoreSlim(5);

        public static async Task CreateTasks(string url, ConcurrentDictionary<string, List<Product>> products)
        {
            var urls = new Dictionary<string,string>
            {
                {"RAM",$"{url}/ram/" },
                {"CPU",$"{url}/processor/" },
                {"MotherBoard",$"{url}/motherboard/" },
                {"GPU",$"{url}/videocard/" },
                {"SSD",$"{url}/ssd/" },
                {"HDD",$"{url}/hard-drive/" }
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 11.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                var parsingTasks = new List<Task>();
                foreach(var item in urls)
                {
                    parsingTasks.Add(GetProduct(httpClient, item.Key, item.Value, products));
                }
                await Task.WhenAll(parsingTasks);
            }
        }
        static async Task GetProduct(HttpClient httpClient, string name, string url, ConcurrentDictionary<string, List<Product>> catalog)
        {
            await semaphore.WaitAsync();
            try
            {
                var products = await GetProductsFromPage(httpClient, url,name);
                catalog.TryAdd(name, products);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                semaphore.Release();
            }
        }
        static async Task<List<Product>> GetProductsFromPage(HttpClient httpClient,string url,string key)
        {
            List<Product> products = new List<Product>();
            bool keepParsing = true;
            int pageCount = 1;
            while (keepParsing)
            {
                try
                {
                    using (var response = await httpClient.GetAsync(url + $"?page={pageCount}")) 
                    {
                        Console.WriteLine($"Waiting for response at page {pageCount} - {key}");
                        if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            keepParsing = false;
                            break;
                        }
                        var html = await response.Content.ReadAsStringAsync();
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(html);
                        var productNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'product-item__inner')]");

                        if (productNodes == null || productNodes.Count == 0)
                        {
                            keepParsing = false;
                            Console.WriteLine($"End of parse - {key}");
                            break;
                        }
                        if (productNodes != null)
                        {
                            foreach (var productNode in productNodes)
                            {
                                string name = productNode.GetAttributeValue("data-prod-name", "Назву не знайдено");
                                string brand = productNode.GetAttributeValue("data-prod-brand", "Назву не знайдено");
                                int price = productNode.GetAttributeValue("data-prod-price", 0);
                                int type = productNode.GetAttributeValue("data-hd-id_category", 0);

                                products.Add(new Product(name, price, brand, type));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                pageCount++;
                await Task.Delay(1000);
            }
            return products;
        }
    }
}
