using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StellarBoocks.Entities;
using StellarBoocks.Data;
using StellarBoocks.DTOs;

namespace StellarBoocks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly StellarBocksApplicationDbContext _context;

        public ActivitiesController(StellarBocksApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivities()
        {
            var activities = await _context.Activities
                .Include(a => a.Tale)
                .ToListAsync();

            return Ok(activities);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            var activity = await _context.Activities
                .Include(a => a.Tale)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return NotFound($"Activity with ID {id} not found.");

            return Ok(activity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var activity = new Activity
            {
                TaleId = dto.TaleId,
                ActivityType = dto.ActivityType,
                Description = dto.Description,
                MultimediaResource = dto.MultimediaResource
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return Ok(new { id = activity.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateActivityDto dto)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return NotFound($"Activity with ID {id} not found.");

            activity.TaleId = dto.TaleId;
            activity.ActivityType = dto.ActivityType;
            activity.Description = dto.Description;
            activity.MultimediaResource = dto.MultimediaResource;

            _context.Activities.Update(activity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return NotFound($"Activity with ID {id} not found.");

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
