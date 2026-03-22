using main.Interfaces;
using main.Models;
using main.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace main.Controlers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScrapeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IScraperService _scraper;
        private readonly IEnumerable<IExporter<List<Product>>> _exporters;
        public ScrapeController(AppDbContext context, IScraperService scraper, IEnumerable<IExporter<List<Product>>> exporters)
        {
            _context = context;
            _scraper = scraper;
            _exporters = exporters;
        }
        [HttpPost("scrape")]
        public async Task<IActionResult> Scrape()
        {
            await _scraper.RunScraperAsync();

            var products = _scraper.GetProductsList();
            foreach (var exporter in _exporters)
                await exporter.ExportAsync(products);

            return Ok(new { Message = "Successfully scraped and saved products", Count = products.Count });
        }
    }
}
