using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    public class HDD : Product
    {
        public HDD(string name, int price, string brand, bool isAvailable, string imageURL, string productURL, int capacity, string formFactor) 
            : base(name, price, brand, isAvailable, imageURL, productURL)
        {
            Capacity = capacity;
            FormFactor = formFactor;
        }
        public int Capacity { get; set; }
        public string FormFactor { get; set; }
    }
}
