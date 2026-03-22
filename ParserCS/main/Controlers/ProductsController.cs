using main.Interfaces;
using main.Models;
using main.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace main.Controlers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
        [HttpPost]
        public async Task<IActionResult> Scrape()
        {
            var scraper = HttpContext.RequestServices.GetRequiredService<IScraperService>();
            var exporters = HttpContext.RequestServices.GetServices<IExporter<List<Product>>>();

            await scraper.RunScraperAsync();

            var products = scraper.GetProductsList();
            foreach (var exporter in exporters)
                await exporter.ExportAsync(products);

            return Ok(new { Message = "Successfully scraped and saved products", Count = products.Count });
        }
    }
}
