using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    public class GPU : Product
    {
        public GPU(string name, int price, string brand, bool isAvailable, string imageURL, string productURL, int memory, string memorytype) 
            : base(name, price, brand, isAvailable, imageURL, productURL)
        {
            Memorytype = memorytype;
            Memory = memory;
        }
        public string Memorytype { get; set; }
        public int Memory { get; set; }
    }
}
