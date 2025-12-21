using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace main.Models
{
    [JsonDerivedType(typeof(CPU), "CPU")]
    [JsonDerivedType(typeof(GPU), "GPU")]
    [JsonDerivedType(typeof(Motherboard), "Motherboard")]
    [JsonDerivedType(typeof(RAM), "RAM")]
    [JsonDerivedType(typeof(SSD), "SSD")]
    [JsonDerivedType(typeof(HDD), "HDD")]
    abstract class Product
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
