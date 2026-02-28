using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using main.Interfaces;
using main.Models;

namespace main.Services
{
    public class TelemartScraper : IScraperService
    {
        private const string url = "https://telemart.ua/ua/city-1482/";
        private readonly Dictionary<ProductType, string> urls = new()
        {
            { ProductType.RAM,$"{url}/ram/" },
            { ProductType.CPU,$"{url}/processor/" },
            { ProductType.Motherboard,$"{url}/motherboard/" },
            { ProductType.GPU,$"{url}/videocard/" },
            { ProductType.SSD,$"{url}/ssd/" },
            { ProductType.HDD,$"{url}/hard-drive/" }
        };


    }
}
