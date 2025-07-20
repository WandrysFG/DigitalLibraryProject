using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using StellarBoocks.Entities;
using StellarBoocks.Data;
using StellarBoocks.DTOs;

namespace StellarBoocks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TalesController : ControllerBase
    {
        private readonly StellarBocksApplicationDbContext _context;

        public TalesController(StellarBocksApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTales()
        {
            var tales = _context.Tales.ToList();
            return Ok(tales);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetTaleById(int id)
        {
            var tale = _context.Tales.FirstOrDefault(t => t.Id == id);
            if (tale == null)
                return NotFound($"Tale with ID {id} not found.");
            return Ok(tale);
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

            _context.Tales.Add(tale);
            _context.SaveChanges();

            return Ok(new { id = tale.Id });
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateTale(int id, [FromBody] Tale request)
        {
            if (request == null)
                return BadRequest("Tale is null or ID mismatch.");

            var existingTale = _context.Tales.FirstOrDefault(t => t.Id == id);
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

            _context.Tales.Update(existingTale);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteTale(int id)
        {
            var tale = _context.Tales.FirstOrDefault(t => t.Id == id);
            if (tale == null)
                return NotFound($"Tale with ID {id} not found.");

            _context.Tales.Remove(tale);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
