using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    class HDD : Product
    {
        public HDD(string name, int price, string brand, bool isAvailable, string url, int capacity, string formfactor) 
            : base(name, price, brand, isAvailable, url)
        {
            Capacity = capacity;
            FormFactor = formfactor;
        }
        public int Capacity { get; set; }
        public string FormFactor { get; set; }
    }
}
