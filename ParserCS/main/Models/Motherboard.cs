using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    public class Motherboard : Product
    {
        public Motherboard(string name, int price, string brand,bool isAvailable, string imageURL, string productURL, string formFactor, string socket,string cpu,string ram) 
            : base(name, price, brand, isAvailable, imageURL, productURL)
        {
            Socket = socket;
            FormFactor = formFactor;
            Cpu = cpu;
            Ram = ram;
        }
        public string Socket { get; set; }
        public string FormFactor { get; set; }
        public string Cpu { get; set; }
        public string Ram { get; set; }
    }
}
