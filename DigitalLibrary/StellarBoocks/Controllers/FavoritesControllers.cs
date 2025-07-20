using Microsoft.AspNetCore.Mvc;
using StellarBoocks.Entities;
using Microsoft.EntityFrameworkCore;
using StellarBoocks.Data;
using StellarBoocks.DTOs;

namespace StellarBoocks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly StellarBocksApplicationDbContext _context;

        public FavoritesController(StellarBocksApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var favorites = await _context.Favorites
                .Include(f => f.User)
                .Include(f => f.Tale)
                .ToListAsync();

            return Ok(favorites);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFavoriteById(int id)
        {
            var favorite = await _context.Favorites
                .Include(f => f.User)
                .Include(f => f.Tale)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (favorite == null)
                return NotFound($"Favorite with ID {id} not found.");

            return Ok(favorite);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFavorite([FromBody] CreateFavoriteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var favorite = new Favorite
            {
                UserId = dto.UserId,
                TaleId = dto.TaleId,
                DateAdded = DateTime.Today
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { id = favorite.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateFavorite(int id, [FromBody] UpdateFavoriteDto dto)
        {
            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null)
                return NotFound($"Favorite with ID {id} not found.");

            favorite.UserId = dto.UserId;
            favorite.TaleId = dto.TaleId;
            favorite.DateAdded = dto.DateAdded;

            _context.Favorites.Update(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite == null)
                return NotFound($"Favorite with ID {id} not found.");

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
