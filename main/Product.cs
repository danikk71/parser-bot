using System;
using System.Collections.Generic;
using System.Text;

namespace main
{
    class Product
    {
        public Product(string name, int price, string brand)
        {
            Name = name;
            Price = price;
            Brand = brand;
        }
        public string Name { get; set; }
        public string Brand { get; set; }
        public int Price { get; set; }
    }
}
