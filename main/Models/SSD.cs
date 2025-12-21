using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    class SSD : Product
    {
        public SSD(string name, int price, string brand, int capacity) : base(name, price, brand)
        {
            Capacity = capacity;
        }
        public int Capacity { get; set; }
    }
}
