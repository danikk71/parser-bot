using main.DTOs;
using main.Models;
using main.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace main.Controlers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavouritesController : Controller
    {
        private readonly AppDbContext _context;
        public FavouritesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddToFavourites([FromBody] FavouriteRequestDto? request)
        {
            if (request == null)
                return BadRequest("Request is null");
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
                return NotFound("Product not found");
            var favourite = new Favourite
            {
                UserId = request.UserId,
                ProductId = request.ProductId
            };

            _context.Favourites.Add(favourite);
            await _context.SaveChangesAsync();
            return Ok(new {Message = "Product successfully added to favourites"});
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetUserFavouritesList([FromQuery] long? id)
        {
            if (id == null)
                return BadRequest("id is null");
            var favourites = await _context.Favourites
                .Where(p => p.UserId == id)
                .ToListAsync();
            return Ok(favourites);
        }

        [HttpGet("is-favourite")]
        public async Task<IActionResult> IsFavourite([FromBody] FavouriteRequestDto? request)
        {
            if (request == null)
                return BadRequest("Request is null");

            bool exists = await _context.Favourites
                .AnyAsync(p => p.UserId == request.UserId && p.ProductId == request.UserId);

            return Ok(exists);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> RemoveFromFavourites([FromBody] FavouriteRequestDto? request)
        {
            if (request == null)
                return BadRequest("Request is null");

            var favouriteToDelete = await _context.Favourites
                .FirstOrDefaultAsync(p => p.UserId == request.UserId && p.ProductId == request.ProductId);

            if (favouriteToDelete == null)
                return NotFound("Product to delete not found");

            _context.Favourites.Remove(favouriteToDelete);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Product successfully deleted from favourites" });
        }

    }
}
