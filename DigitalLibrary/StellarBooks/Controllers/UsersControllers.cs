using Microsoft.AspNetCore.Mvc;
using StellarBooks.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Repositories;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork.Users.GetAllUsersWithFavoritesAndTales();

            var usersWithFavorites = users.Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.UserType,
                u.IsActive,
                u.RegistrationDate,
                Favorites = u.Favorites.Select(f => new
                {
                    f.Tale.Title,
                    f.DateAdded
                }).ToList()
            }).ToList();

            return Ok(usersWithFavorites);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _unitOfWork.Users.GetUserWithFavoritesAndTalesById(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            var userWithFavorites = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.UserType,
                user.IsActive,
                user.RegistrationDate,
                Favorites = user.Favorites.Select(f => new
                {
                    TaleTitle = f.Tale.Title,
                    f.DateAdded
                }).ToList()
            };

            return Ok(userWithFavorites);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
        {
            if (request == null)
                return BadRequest("User cannot be null.");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                UserType = request.UserType,
                IsActive = request.IsActive,
                RegistrationDate = System.DateTime.UtcNow.Date
            };

            user = await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return Ok(new { id = user.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User request)
        {
            if (request == null)
                return BadRequest("User is null or ID mismatch.");

            var existingUser = await _unitOfWork.Users.GetByIdAsync(request.Id);
            if (existingUser == null)
                return NotFound($"User with ID {id} not found.");

            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.Email = request.Email;
            existingUser.Password = request.Password;
            existingUser.UserType = request.UserType;
            existingUser.IsActive = request.IsActive;
            existingUser.RegistrationDate = request.RegistrationDate;

            await _unitOfWork.Users.UpdateAsync(existingUser);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            
            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}