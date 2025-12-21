using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace main
{
    class Parser
    {
        //private static List<string> GenerateURLs(string url)
        //{
            
        //}
        public static async Task<Dictionary<string,List<Product>>> GetProductsFromPage(string url)
        {
            Dictionary<string, List<Product>> products = new Dictionary<string, List<Product>>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 11.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                
                bool keepParsing = true;
                int pageCount = 1;
                while (keepParsing)
                {
                    try
                    {
                        using (var response = await httpClient.GetAsync(url + $"?page={pageCount}")) 
                        {
                            Console.WriteLine($"Waiting for response at page {pageCount}");
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
                                Console.WriteLine("End of parse");
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

                                    string productType = string.Empty;
                                    switch (type)
                                    {
                                        case 403:
                                            productType = "RAM"; break;
                                        default:
                                            productType = "Other"; break;
                                    }

                                    if (!products.ContainsKey(productType)) products.Add(productType, new List<Product>());
                                    products[productType].Add(new Product(name, price, brand));
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
}
