using Microsoft.AspNetCore.Mvc;
using StellarBooks.Entities;
using StellarBooks.Data;
using StellarBooks.DTOs;

namespace StellarBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly StellarBocksApplicationDbContext _context;

        public UsersController(StellarBocksApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
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

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { id = user.Id });
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, [FromBody] User request)
        {
            if (request == null)
                return BadRequest("User is null or ID mismatch.");

            var existingUser = _context.Users.FirstOrDefault(u => u.Id == id);
            if (existingUser == null)
                return NotFound($"User with ID {id} not found.");

            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.Email = request.Email;
            existingUser.Password = request.Password;
            existingUser.UserType = request.UserType;
            existingUser.IsActive = request.IsActive;
            existingUser.RegistrationDate = request.RegistrationDate;

            _context.Users.Update(existingUser);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            _context.Users.Remove(user);
            _context.SaveChanges();

            return NoContent();
        }
    }
}