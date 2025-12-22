using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using main.Models;

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
        static string SearchAttribute(string attribute,HtmlNodeCollection? htmlNodes) //також можна буде добавити якийсь Regex аргумент , щоб наприклад брати пам'ять в ГБ/ТБ
        {
            if (htmlNodes == null) return "htmlNodes пустий";
            foreach(var row in htmlNodes)
            {
                var labelNode = row.SelectSingleNode(".//span[contains(@class,'product-short-char__item__label')]");
                if (labelNode != null && labelNode.InnerText.Contains(attribute, StringComparison.OrdinalIgnoreCase))
                {
                    var valueNode = row.SelectSingleNode(".//span[contains(@class,'product-short-char__item__value')]");

                    if (valueNode != null)
                    {
                        return valueNode.InnerText.Trim();
                    }
                }
            }
            return $"{attribute} Атрибут не знайдено";
        }
        static int ParseCapacity(string capacity)
        {
            string capacityNumber = Regex.Replace(capacity, @"\D+", "");
            if (!int.TryParse(capacityNumber, out int number)) return 0;

            if (capacity.Contains("TB", StringComparison.OrdinalIgnoreCase))
            {
                return number * 1024;
            }
            return number;
        }
        static int GetIntAttribule(string attribute,HtmlNodeCollection? htmlNodes)
        {
            string value = SearchAttribute(attribute, htmlNodes);
            string intvalue = Regex.Replace(value, @"[^\d]", "");
            return string.IsNullOrEmpty(intvalue) ? 0 : int.Parse(intvalue);
        }
        static Product? CreateProduct(HtmlNode? productNode)
        {
            if (productNode == null)
            {
                Console.WriteLine("Предмет не знайдено");
                return null;
            }
            string name = productNode.GetAttributeValue("data-prod-name", "Назву не знайдено");
            string brand = productNode.GetAttributeValue("data-prod-brand", "Назву не знайдено");
            int price = productNode.GetAttributeValue("data-prod-price", 0);
            int type = productNode.GetAttributeValue("data-hd-id_category", 0);
            bool isAvailable = !productNode.ParentNode.GetAttributeValue("class","").Contains("product-item--not-available");

            var attributes = productNode.SelectNodes(".//div[contains(@class, 'product-short-char__item')]");

            switch (type)
            {
                case 397:
                    return new GPU(name, price, brand, isAvailable,
                        GetIntAttribule("обсяг", attributes),
                        SearchAttribute("тип", attributes));
                case 398:
                    return new CPU(name, price, brand, isAvailable,
                        GetIntAttribule("кількість", attributes),
                        SearchAttribute("роз'єм", attributes));
                case 399:
                    return new HDD(name, price, brand, isAvailable,
                        ParseCapacity(SearchAttribute("обсяг", attributes)),
                        SearchAttribute("форм-фактор", attributes));
                case 400:
                    return new Motherboard(name, price, brand, isAvailable,
                        SearchAttribute("форм-фактор", attributes),
                        SearchAttribute("роз'єм", attributes),
                        SearchAttribute("тип", attributes),
                        SearchAttribute("сумісні", attributes));
                case 403:
                    return new RAM(name, price, brand, isAvailable,
                        GetIntAttribule("Обсяг одного модуля", attributes), 
                        SearchAttribute("тип", attributes),
                        GetIntAttribule("частота",attributes));
                case 407:
                    return new HDD(name, price, brand, isAvailable,
                        ParseCapacity(SearchAttribute("обсяг", attributes)),
                        SearchAttribute("форм-фактор", attributes));
                default:
                    return null;
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
                        foreach (var productNode in productNodes)
                        {
                            products.Add(CreateProduct(productNode) ?? new RAM("1",1,"1",false,1,"1",1));  //заглушка тимчасова на випадок null
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"oh shit , error : {ex.Message}");
                }
                pageCount++;
                await Task.Delay(1000);
            }
            return products;
        }
    }
}
