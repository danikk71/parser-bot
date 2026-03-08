using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace main.Models
{
    public class PriceHistory
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Price { get; set; }
        public DateTime DateRecorded { get; set; }
    }
}
