using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using main.Models;
using main.Interfaces;

namespace main.Services.Storage
{
    public class DbStorageService : IExporter<List<Product>>
    {
        private readonly AppDbContext _context;

        public DbStorageService(AppDbContext context)
        {
            _context = context;
        }
        public async Task ExportAsync(List<Product> products)
        {
            try
            {
                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при запису до бази : {ex.Message}");
            }
        }
    }
}
