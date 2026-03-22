using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using main.Models;
using main.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            var today = DateTime.UtcNow.Date;

            foreach(var item in products)
            {
                try
                {
                    var existingProduct = await _context.Products
                        .FirstOrDefaultAsync(p => p.ProductURL == item.ProductURL);

                    if(existingProduct != null)
                    {
                        existingProduct.Price = item.Price;
                        existingProduct.IsAvailable = item.IsAvailable;
                        existingProduct.ImageURL = item.ImageURL;

                        bool isRecordedToday = existingProduct.PriceHistories
                            .Any(ph => ph.DateRecorded.Date == today);

                        if(!isRecordedToday && item.IsAvailable)
                        {
                            existingProduct.PriceHistories.Add(new PriceHistory
                            {
                                Price = item.Price,
                                DateRecorded = today
                            });
                        }
                    }
                    else
                    {
                        if (item.IsAvailable)
                        {
                            existingProduct.PriceHistories.Add(new PriceHistory
                            {
                                Price = item.Price,
                                DateRecorded = today
                            });
                        }
                        await _context.Products.AddAsync(item);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при запису до бази : {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
