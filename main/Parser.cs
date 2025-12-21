using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace main
{
    internal class Parser
    {
        static readonly string url = "https://telemart.ua/ua/city-1482/ram/";
        public static async Task<List<Product>> GetProducts()
        {
            List<Product> products = new List<Product>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 11.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                try
                {
                    var html = await httpClient.GetStringAsync(url);
                    Console.WriteLine("Waiting for response");
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    var productNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'product-item__inner')]");

                    if (productNodes != null)
                    {
                        foreach (var productNode in productNodes)
                        {
                            string name = productNode.GetAttributeValue("data-prod-name", "Назву не знайдено");
                            string price = productNode.GetAttributeValue("data-prod-price", "0");
                            int type = productNode.GetAttributeValue("data-hd-id_category", 0);
                            switch (type)
                            {
                                case 403: products.Add(new Product(name, int.Parse(price), "Оперативна пам`ять")); break;
                                default: products.Add(new Product(name, int.Parse(price), "Невідомий продукт")); break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return products;
            }
        }
    }
}
