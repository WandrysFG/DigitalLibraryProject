using Microsoft.AspNetCore.Mvc;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Repositories;
using StellarBooks.Infrastructure.Interface;
using StellarBooks.Applications.DTOs;
using StellarBooks.Application.Interfaces;
using StellarBooks.Application.Services;

namespace StellarBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TalesController : ControllerBase
    {
        private readonly ITaleService _taleService;

        public TalesController(ITaleService taleService)
        {
            _taleService = taleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTales()
        {
            return Ok(await _taleService.GetTales());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTaleById(int id)
        {
            return Ok(await _taleService.GetTaleById(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTale([FromBody] CreateTaleDto request)
        {
            var responseId = await _taleService.CreateTale(request);
            return Ok(new { id = responseId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTale(int id, [FromBody] UpdateTaleDto request)
        {
            await _taleService.UpdateTale(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTale(int id)
        {
            await _taleService.DeleteTale(id);
            return NoContent();
        }
    }
}
