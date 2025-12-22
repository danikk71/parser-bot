using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    class GPU : Product
    {
        public GPU(string name, int price, string brand, bool isAvailable, string url, int memory, string memorytype) 
            : base(name, price, brand, isAvailable, url)
        {
            Memorytype = memorytype;
            Memory = memory;
        }
        public string Memorytype { get; set; }
        public int Memory { get; set; }
    }
}
