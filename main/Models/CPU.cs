using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    class CPU : Product
    {
        public CPU(string name, int price, string brand, bool isAvailable, int cores, string socket) : base(name, price, brand, isAvailable)
        {
            Socket = socket;
            Cores = cores;
        }
        public string Socket { get; set; }
        public int Cores { get; set; }
    }
}
