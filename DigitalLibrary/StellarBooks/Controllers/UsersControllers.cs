using Microsoft.AspNetCore.Mvc;
using StellarBooks.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Data;
using StellarBooks.Infrastructure.Repositories;

namespace StellarBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UsersController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAllUsersWithFavoritesAndTales();

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
            var user = await _userRepository.GetUserWithFavoritesAndTalesById(id);
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

            user = await _userRepository.AddAsync(user);

            return Ok(new { id = user.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User request)
        {
            if (request == null)
                return BadRequest("User is null or ID mismatch.");

            var existingUser = await _userRepository.GetByIdAsync(request.Id);
            if (existingUser == null)
                return NotFound($"User with ID {id} not found.");

            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.Email = request.Email;
            existingUser.Password = request.Password;
            existingUser.UserType = request.UserType;
            existingUser.IsActive = request.IsActive;
            existingUser.RegistrationDate = request.RegistrationDate;

            await _userRepository.UpdateAsync(existingUser);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            
            await _userRepository.DeleteAsync(user);
            return NoContent();
        }
    }
}