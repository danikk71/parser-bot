using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace main.Models
{
    public class Favourite
    {
        public int Id { get; set; }
        public long UserId { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
