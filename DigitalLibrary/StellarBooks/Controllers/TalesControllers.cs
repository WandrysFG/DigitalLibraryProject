using Microsoft.AspNetCore.Mvc;
using StellarBooks.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Repositories;

namespace StellarBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TalesController : ControllerBase
    {
        private readonly TaleRepository _taleRepository;

        public TalesController(TaleRepository taleRepository)
        {
            _taleRepository = taleRepository;
        }

        [HttpGet]
        public IActionResult GetTales()
        {
            var tales = _taleRepository.GetAllTales()
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.RecommendedAge,
                    t.Theme,
                    t.Content,
                    t.CoverImage,
                    t.NarrationAudio,
                    t.IsAvailable,
                    t.PublicationDate
                })
                .ToList();

            return Ok(tales);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetTaleById(int id)
        {
            var tale = _taleRepository.GetTaleById(id);
            if (tale == null)
                return NotFound($"Tale with ID {id} not found.");

            var result = new
            {
                tale.Id,
                tale.Title,
                tale.RecommendedAge,
                tale.Theme,
                tale.Content,
                tale.CoverImage,
                tale.NarrationAudio,
                tale.IsAvailable,
                tale.PublicationDate
            };

            return Ok(result);
        }

        [HttpPost]
        public IActionResult CreateTale([FromBody] CreateTaleDto request)
        {
            if (request == null)
                return BadRequest("Tale cannot be null.");

            var tale = new Tale
            {
                Title = request.Title,
                RecommendedAge = request.RecommendedAge,
                Theme = request.Theme,
                Content = request.Content,
                CoverImage = request.CoverImage,
                NarrationAudio = request.NarrationAudio,
                IsAvailable = request.IsAvailable,
                PublicationDate = System.DateTime.UtcNow.Date
            };

            _taleRepository.AddTale(tale);

            return Ok(new { id = tale.Id });
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateTale(int id, [FromBody] Tale request)
        {
            if (request == null)
                return BadRequest("Tale is null or ID mismatch.");

            var existingTale = _taleRepository.GetTaleById(id);
            if (existingTale == null)
                return NotFound($"Tale with ID {id} not found.");

            existingTale.Title = request.Title;
            existingTale.RecommendedAge = request.RecommendedAge;
            existingTale.Theme = request.Theme;
            existingTale.Content = request.Content;
            existingTale.CoverImage = request.CoverImage;
            existingTale.NarrationAudio = request.NarrationAudio;
            existingTale.IsAvailable = request.IsAvailable;
            existingTale.PublicationDate = request.PublicationDate;

            _taleRepository.UpdateTale(existingTale);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteTale(int id)
        {
            var tale = _taleRepository.GetTaleById(id);
            if (tale == null)
                return NotFound($"Tale with ID {id} not found.");

            _taleRepository.DeleteTale(id);

            return NoContent();
        }
    }
}
