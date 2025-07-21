using Microsoft.AspNetCore.Mvc;
using StellarBooks.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace StellarBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly FavoriteRepository _favoriteRepository;

        public FavoritesController(FavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        [HttpGet]
        public IActionResult GetFavorites()
        {
            var favorites = _favoriteRepository.GetAllFavorites()
                .Select(f => new
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
                        f.Tale.RecommendedAge
                    }
                }).ToList();

            return Ok(favorites);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetFavoriteById(int id)
        {
            var favorite = _favoriteRepository.GetFavoriteById(id);
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
                    favorite.Tale.RecommendedAge
                }
            };

            return Ok(result);
        }

        [HttpPost]
        public IActionResult CreateFavorite([FromBody] CreateFavoriteDto dto)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            //var favorite = new Favorite
            //{
            //    UserId = dto.UserId,
            //    TaleId = dto.TaleId,
            //    DateAdded = System.DateTime.UtcNow.Date
            //};

            //_favoriteRepository.AddFavorite(favorite);

            //return Ok(new { id = favorite.Id });

            var favorite = new Favorite
            {
                UserId = dto.UserId,
                TaleId = dto.TaleId,
                DateAdded = DateTime.UtcNow.Date
            };

            _favoriteRepository.AddFavorite(favorite);

            return Ok(new { id = favorite.Id });
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateFavorite(int id, [FromBody] UpdateFavoriteDto dto)
        {
            var favorite = _favoriteRepository.GetFavoriteById(id);
            if (favorite == null)
                return NotFound($"Favorite with ID {id} not found.");

            favorite.UserId = dto.UserId;
            favorite.TaleId = dto.TaleId;
            favorite.DateAdded = dto.DateAdded;

            _favoriteRepository.UpdateFavorite(favorite);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteFavorite(int id)
        {
            var favorite = _favoriteRepository.GetFavoriteById(id);
            if (favorite == null)
                return NotFound($"Favorite with ID {id} not found.");

            _favoriteRepository.DeleteFavorite(id);

            return NoContent();
        }
    }
}
