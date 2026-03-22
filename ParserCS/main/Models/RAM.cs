using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    public class RAM : Product
    {
        public RAM(string name, int price, string brand,bool isAvailable,string imageURL, string productURL, int memory,string memoryType,int frequency) 
            : base(name, price, brand, isAvailable, imageURL, productURL)
        {
            Memory = memory;
            MemoryType = memoryType;
            Frequency = frequency;
        }
        public string MemoryType { get; set; }
        public int Memory { get; set; }
        public int Frequency { get; set; }
    }
}
