using Microsoft.AspNetCore.Mvc;
using StellarBooks.Domain.Entities;
using StellarBooks.DTOs;
using StellarBooks.Infrastructure.Interface;
using StellarBooks.Infrastructure.Repositories;

namespace StellarBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivitiesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivities()
        {
            var activities = await _unitOfWork.Activities.GetAllAsync();

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
        public async Task<IActionResult> GetActivityById(int id)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);

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

            var id = await _unitOfWork.Activities.AddAsync(activity);
            await _unitOfWork.CompleteAsync();

            return Ok(new { id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateActivityDto dto)
        {
            var existing = await _unitOfWork.Activities.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Activity with ID {id} not found.");

            existing.TaleId = dto.TaleId;
            existing.ActivityType = dto.ActivityType;
            existing.Description = dto.Description;
            existing.MultimediaResource = dto.MultimediaResource;

            await _unitOfWork.Activities.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);
            if (activity == null)
                return NotFound($"Activity with ID {id} not found.");

            await _unitOfWork.Activities.DeleteAsync(activity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}