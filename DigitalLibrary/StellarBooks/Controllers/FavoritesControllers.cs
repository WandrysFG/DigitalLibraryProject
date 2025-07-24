using Microsoft.AspNetCore.Mvc;
using StellarBooks.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavoritesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var favorites = await _unitOfWork.Favorites.GetAllWithUserAndTale();
                var result = favorites.Select(f => new
                {
                f.Id,
                    f.DateAdded,
                    User = new
                    {
                        f.User.Id,
                        f.User.FirstName,
                        f.User.LastName
                    },
                    Tale = new
                    {
                        f.Tale.Id,
                        f.Tale.Title,
                    }
                }).ToList();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFavoriteById(int id)
        {
            var favorite = await _unitOfWork.Favorites.GetByIdWithUserAndTale(id);
            if (favorite == null)
                return NotFound($"Favorite with ID {id} not found.");

            var result = new
            {
                favorite.Id,
                favorite.DateAdded,
                User = new
                {
                    favorite.User.Id,
                    favorite.User.FirstName,
                    favorite.User.LastName
                },
                Tale = new
                {
                    favorite.Tale.Id,
                    favorite.Tale.Title,
                }
            };

            return Ok(result);
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
                DateAdded = DateTime.UtcNow.Date
            };

            await _unitOfWork.Favorites.AddAsync(favorite);
            await _unitOfWork.CompleteAsync();

            return Ok(new { id = favorite.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateFavorite(int id, [FromBody] UpdateFavoriteDto dto)
        {
            var favorite = await _unitOfWork.Favorites.GetByIdAsync(id);
            if (favorite == null)
                return NotFound($"Favorite with ID {id} not found.");

            favorite.UserId = dto.UserId;
            favorite.TaleId = dto.TaleId;
            favorite.DateAdded = dto.DateAdded;

            await _unitOfWork.Favorites.UpdateAsync(favorite);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var favorite = await _unitOfWork.Favorites.GetByIdAsync(id);
            if (favorite == null)
                return NotFound($"Favorite with ID {id} not found.");

            _unitOfWork.Favorites.DeleteAsync(favorite);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
