using System.Xml.Linq;
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
        [HttpGet("all")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("name")]
        public async Task<IActionResult> GetProductByName([FromQuery] string? name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("Name can`t be empty");
            var products = await _context.Products
                .Where(p => EF.Functions.ILike(p.Name, $"%{name}%") && p.IsAvailable)
                .ToListAsync();
            return Ok(products);
        }

        [HttpGet("type")]
        public async Task<IActionResult> GetProductByType([FromQuery] ProductType? type)
        {
            if(type == null)
                return BadRequest("Type can`t be empty");
            var products = await _context.Products
                .Where(p => EF.Property<ProductType>(p, "Type") == type && p.IsAvailable)
                .ToListAsync();
            return Ok(products);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetProductById([FromQuery] int? id)
        {
            if (id == null)
                return BadRequest("Id can`t be empty");
            var product = await _context.Products
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            return Ok(product);
        }

        [HttpGet("brand")]
        public async Task<IActionResult> GetProductByBrand([FromQuery] string? brand)
        {
            if (string.IsNullOrEmpty(brand))
                return BadRequest("Brand can`t be empty");
            var products = await _context.Products
                .Where(p => EF.Functions.ILike(p.Name, $"%{brand}%") && p.IsAvailable)
                .ToListAsync();
            return Ok(products);
        }
    }
}
