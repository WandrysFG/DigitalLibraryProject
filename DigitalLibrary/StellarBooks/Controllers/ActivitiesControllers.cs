using Microsoft.AspNetCore.Mvc;
using StellarBooks.Application.Interfaces;
using StellarBooks.Applications.DTOs;

namespace StellarBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivities()
        {
            return Ok(await _activityService.GetActivities());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            return Ok(await _activityService.GetActivityById(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto request)
        {
            var responseId = await _activityService.CreateActivity(request);
            return Ok(new { id = responseId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateActivityDto request)
        {
            await _activityService.UpdateActivity(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            await _activityService.DeleteActivity(id);
            return NoContent();
        }
    }
}