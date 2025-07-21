using Microsoft.AspNetCore.Mvc;
using StellarBooks.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Repositories;

namespace StellarBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly ActivityRepository _activityRepository;

        public ActivitiesController(ActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        [HttpGet]
        public IActionResult GetActivities()
        {
            var activities = _activityRepository.GetAllActivities();

            var result = activities.Select(a => new
            {
                a.Id,
                a.TaleId,
                a.ActivityType,
                a.Description,
                a.MultimediaResource,
                Tale = a.Tale == null ? null : new
                {
                    a.Tale.Id,
                    a.Tale.Title,
                    a.Tale.RecommendedAge
                }
            });

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetActivityById(int id)
        {
            var activity = _activityRepository.GetActivityById(id);

            if (activity == null)
                return NotFound($"Activity with ID {id} not found.");

            var result = new
            {
                activity.Id,
                activity.TaleId,
                activity.ActivityType,
                activity.Description,
                activity.MultimediaResource,
                Tale = activity.Tale == null ? null : new
                {
                    activity.Tale.Id,
                    activity.Tale.Title,
                    activity.Tale.RecommendedAge
                }
            };

            return Ok(result);
        }

        [HttpPost]
        public IActionResult CreateActivity([FromBody] CreateActivityDto dto)
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

            var id = _activityRepository.AddActivity(activity);

            return Ok(new { id });
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateActivity(int id, [FromBody] UpdateActivityDto dto)
        {
            var existing = _activityRepository.GetActivityById(id);
            if (existing == null)
                return NotFound($"Activity with ID {id} not found.");

            existing.TaleId = dto.TaleId;
            existing.ActivityType = dto.ActivityType;
            existing.Description = dto.Description;
            existing.MultimediaResource = dto.MultimediaResource;

            _activityRepository.UpdateActivity(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteActivity(int id)
        {
            var existing = _activityRepository.GetActivityById(id);
            if (existing == null)
                return NotFound($"Activity with ID {id} not found.");

            _activityRepository.DeleteActivity(id);
            return NoContent();
        }
    }
}
