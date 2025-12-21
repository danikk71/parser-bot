using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    class RAM : Product
    {
        public RAM(string name, int price, string brand,int memory,string memory_type,int frequency) : base(name, price, brand)
        {
            Memory = memory;
            Memory_type = memory_type;
            Frequency = frequency;
        }
        public string Memory_type { get; set; }
        public int Memory { get; set; }
        public int Frequency { get; set; }
    }
}
