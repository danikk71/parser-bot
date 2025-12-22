using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    class SSD : Product
    {
        public SSD(string name, int price, string brand, bool isAvailable, int capacity) : base(name, price, brand, isAvailable)
        {
            Capacity = capacity;
        }
        public int Capacity { get; set; }
    }
}
