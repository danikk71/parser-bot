using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using main.Models;

namespace main.Interfaces
{
    public interface IScraperService
    {
        public Task RunScraperAsync();
        public List<Product> GetProductsList();
    }
}
