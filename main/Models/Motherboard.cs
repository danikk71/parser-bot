using System;
using System.Collections.Generic;
using System.Text;

namespace main.Models
{
    class Motherboard : Product
    {
        public Motherboard(string name, int price, string brand, string formfactor, string socket,string cpu,string ram) : base(name, price, brand)
        {
            Socket = socket;
            FormFactor = formfactor;
            CPU = cpu;
            RAM = ram;
        }
        public string Socket { get; set; }
        public string FormFactor { get; set; }
        public string CPU { get; set; }
        public string RAM { get; set; }
    }
}
