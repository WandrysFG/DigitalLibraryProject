using Microsoft.AspNetCore.Mvc;
using StellarBooks.Application.Interfaces;
using StellarBooks.Applications.DTOs;

namespace StellarBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            return Ok(await _favoriteService.GetFavorites());
        }

        [HttpGet]
        [Route("with-user-tale")]
        public async Task<IActionResult> GetAllWithUserAndTale()
        {
            return Ok(await _favoriteService.GetAllWithUserAndTale());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFavoriteById(int id)
        {
            return Ok(await _favoriteService.GetFavoriteById(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateFavorite([FromBody] CreateFavoriteDto request)
        {
            var responseId = await _favoriteService.CreateFavorite(request);
            return Ok(new { id = responseId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateFavorite(int id, [FromBody] UpdateFavoriteDto request)
        {
            await _favoriteService.UpdateFavorite(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            await _favoriteService.DeleteFavorite(id);
            return NoContent();
        }
    }
}
