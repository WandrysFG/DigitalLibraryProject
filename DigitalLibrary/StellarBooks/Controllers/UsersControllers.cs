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
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetAllUsers();
            return Ok(users);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");
            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserDto request)
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

            var userId = _userRepository.AddUser(user);

            return Ok(new { id = userId });
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, [FromBody] User request)
        {
            if (request == null)
                return BadRequest("User is null or ID mismatch.");

            var existingUser = _userRepository.GetUserById(request.Id);
            if (existingUser == null)
                return NotFound($"User with ID {id} not found.");

            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.Email = request.Email;
            existingUser.Password = request.Password;
            existingUser.UserType = request.UserType;
            existingUser.IsActive = request.IsActive;
            existingUser.RegistrationDate = request.RegistrationDate;

            _userRepository.UpdateUser(existingUser);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            _userRepository.DeleteUser(id);
            return NoContent();
        }
    }
}