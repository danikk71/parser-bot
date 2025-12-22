using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    class CPU : Product
    {
        public CPU(string name, int price, string brand, bool isAvailable, string url, int cores, string socket) 
            : base(name, price, brand, isAvailable, url)
        {
            Socket = socket;
            Cores = cores;
        }
        public string Socket { get; set; }
        public int Cores { get; set; }
    }
}
