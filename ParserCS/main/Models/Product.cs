using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace main.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(CPU), nameof(ProductType.CPU))]
    [JsonDerivedType(typeof(GPU), nameof(ProductType.GPU))]
    [JsonDerivedType(typeof(Motherboard), nameof(ProductType.Motherboard))]
    [JsonDerivedType(typeof(RAM), nameof(ProductType.RAM))]
    [JsonDerivedType(typeof(SSD), nameof(ProductType.SSD))]
    [JsonDerivedType(typeof(HDD), nameof(ProductType.HDD))]
    public abstract class Product
    {
        protected Product(string name, int price, string brand,bool isAvailable,string imageURL,string productURL)
        {
            Name = name;
            Price = price;
            Brand = brand;
            IsAvailable = isAvailable;
            ImageURL = imageURL;
            ProductURL = productURL;
        }
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public int Price { get; set; }
        public bool IsAvailable { get; set; }
        public string ImageURL { get; set; }
        public string ProductURL { get; set; }
    }
}
