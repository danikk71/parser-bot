using main.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace main.Controlers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceHistoryController : Controller
    {
        private readonly AppDbContext _context;
        public PriceHistoryController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("prices")]
        public async Task<IActionResult> GetProductPrices(int? productId)
        {
            if (productId == null)
                return BadRequest("product id is null");
            var products = await _context.PriceHistories
                .Where(p => p.ProductId == productId)
                .ToListAsync();

            return Ok(products);
        }
    }
}
