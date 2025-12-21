using System;
using System.Collections.Generic;
using System.Text;

namespace main
{
    class Product
    {
        public Product(string name, int price, string type)
        {
            Name = name;
            Price = price;
            Type = type;
        }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Type { get; set; }
    }
}
