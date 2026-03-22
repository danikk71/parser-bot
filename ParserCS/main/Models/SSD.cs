using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    public class SSD : Product
    {
        public SSD(string name, int price, string brand, bool isAvailable, string imageURL, string productURL, int capacity) 
            : base(name, price, brand, isAvailable, imageURL, productURL)
        {
            Capacity = capacity;
        }
        public int Capacity { get; set; }
    }
}
