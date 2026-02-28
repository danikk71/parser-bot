using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using main.Interfaces;
using main.Models;

namespace main.Services
{
    public class TelemartMapper : IMapper<HtmlNode, Product>
    {
        public Product? Map(HtmlNode sourceNode)
        {
            if (sourceNode == null)
            {
                Console.WriteLine("Предмет не знайдено");
                return null;
            }
            bool isAvailable = !sourceNode.GetAttributeValue("class", "").Contains("product-item--not-available");
            string imageURL = sourceNode.
                SelectSingleNode(".//div[contains(@class,'swiper-slide')]")?.
                SelectSingleNode(".//img")?.
                GetAttributeValue("src", "Фотографія не знайдена") ?? "Фотографія не знайдена";

            string URL = sourceNode.
                SelectSingleNode(".//div[contains(@class,'product-item__title')]")?.
                SelectSingleNode(".//a")?.
                GetAttributeValue("href", "Посилання не знайдено") ?? "Посилання не знайдено";

            var dataNode = sourceNode.SelectSingleNode(".//div[contains(@class,'product-item__inner')]");
            if (dataNode == null) return null;

            string name = dataNode.GetAttributeValue("data-prod-name", "Назву не знайдено");
            string brand = dataNode.GetAttributeValue("data-prod-brand", "Назву не знайдено");
            int price = dataNode.GetAttributeValue("data-prod-price", 0);
            ProductType type = (ProductType)dataNode.GetAttributeValue("data-hd-id_category", 0);

            var attributes = dataNode.SelectNodes(".//div[contains(@class, 'product-short-char__item')]");

            switch (type)
            {
                case ProductType.GPU:
                    return new GPU(name, price, brand, isAvailable, imageURL, URL,
                        GetIntAttribule("обсяг", attributes),
                        SearchAttribute("тип", attributes));
                case ProductType.CPU:
                    return new CPU(name, price, brand, isAvailable, imageURL, URL,
                        GetIntAttribule("кількість", attributes),
                        SearchAttribute("роз'єм", attributes));
                case ProductType.HDD:
                    return new HDD(name, price, brand, isAvailable, imageURL, URL,
                        ParseCapacity(SearchAttribute("обсяг", attributes)),
                        SearchAttribute("форм-фактор", attributes));
                case ProductType.Motherboard:
                    return new Motherboard(name, price, brand, isAvailable, imageURL, URL,
                        SearchAttribute("форм-фактор", attributes),
                        SearchAttribute("роз'єм", attributes),
                        SearchAttribute("тип", attributes),
                        SearchAttribute("сумісні", attributes));
                case ProductType.RAM:
                    return new RAM(name, price, brand, isAvailable, imageURL, URL,
                        GetIntAttribule("Обсяг одного модуля", attributes),
                        SearchAttribute("тип", attributes),
                        GetIntAttribule("частота", attributes));
                case ProductType.SSD:
                    return new SSD(name, price, brand, isAvailable, imageURL, URL,
                        ParseCapacity(SearchAttribute("обсяг", attributes)));
                default:
                    return null;
            }
        }

        private int GetIntAttribule(string attribute, HtmlNodeCollection? htmlNodes)
        {
            string value = SearchAttribute(attribute, htmlNodes);
            string intvalue = Regex.Replace(value, @"[^\d]", "");
            return string.IsNullOrEmpty(intvalue) ? 0 : int.Parse(intvalue);
        }

        private string SearchAttribute(string attribute, HtmlNodeCollection? htmlNodes)
        {
            if (htmlNodes == null) return "htmlNodes пустий";
            foreach (var row in htmlNodes)
            {
                var labelNode = row.SelectSingleNode(".//span[contains(@class,'product-short-char__item__label')]");
                if (labelNode != null && labelNode.InnerText.Contains(attribute, StringComparison.OrdinalIgnoreCase))
                {
                    var valueNode = row.SelectSingleNode(".//span[contains(@class,'product-short-char__item__value')]");

                    if (valueNode != null)
                        return valueNode.InnerText.Trim();
                }
            }
            return $"{attribute} Атрибут не знайдено";
        }

        private int ParseCapacity(string capacity)
        {
            string capacityNumber = Regex.Replace(capacity, @"\D+", "");
            if (!int.TryParse(capacityNumber, out int number)) 
                return 0;

            if (capacity.Contains("TB", StringComparison.OrdinalIgnoreCase))
                return number * 1024;
            return number;
        }
    }
}
