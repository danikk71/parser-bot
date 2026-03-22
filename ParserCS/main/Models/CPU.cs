using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    public class CPU : Product
    {
        public CPU(string name, int price, string brand, bool isAvailable, string imageURL, string productURL, int cores, string socket) 
            : base(name, price, brand, isAvailable, imageURL, productURL)
        {
            Socket = socket;
            Cores = cores;
        }
        public string Socket { get; set; }
        public int Cores { get; set; }
    }
}
