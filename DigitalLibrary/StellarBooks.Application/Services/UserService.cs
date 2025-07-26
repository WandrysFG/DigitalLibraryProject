using StellarBooks.Application.Interfaces;
using StellarBooks.Applications.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UpdateUserDto>> GetUsers()
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            var result = users.Select(u => new UpdateUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                UserType = u.UserType,
                IsActive = u.IsActive,
                RegistrationDate = u.RegistrationDate,
            }).ToList();

            return result;
        }

        public async Task<List<UpdateUserDto>> GetAllUsersWithFavorites()
        {
            var users = await _unitOfWork.Users.GetAllUsersWithFavorites();

            var usersWithFavorites = users.Select(u => new UpdateUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                UserType = u.UserType,
                IsActive = u.IsActive,
                RegistrationDate = u.RegistrationDate,
                Favorites = u.Favorites?.Select(f => new UpdateFavoriteDto
                {
                    Id = f.Id,
                    TaleId = f.TaleId,
                    DateAdded = f.DateAdded,
                    UserId = f.UserId
                }).ToList()
            }).ToList();

            return usersWithFavorites;
        }

        public async Task<UpdateUserDto> GetUserById(int id)
        {
            var user = await _unitOfWork.Users.GetUserWithFavoritesById(id);
            if (user == null)
            {
                throw new Exception($"User with ID {id} not found.");
            }

            var response = new UpdateUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserType = user.UserType,
                IsActive = user.IsActive,
                RegistrationDate = user.RegistrationDate,
                Favorites = user.Favorites?.Select(f => new UpdateFavoriteDto
                {
                    Id = f.Id,
                    TaleId = f.TaleId,
                    DateAdded = f.DateAdded,
                    UserId = f.UserId
                }).ToList()
            };

            return response;
        }

        public async Task<int> CreateUser(CreateUserDto request)
        {
            if (request == null)
            {
                throw new Exception("User cannot be null.");
            }

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

            return user.Id;
        }

        public async Task UpdateUser(int id, UpdateUserDto request)
        {
            if (request == null)
            {
                throw new Exception("User is null or ID mismatch.");
            }

            var existingUser = await _unitOfWork.Users.GetByIdAsync(request.Id);

            if (existingUser == null)
            {
                throw new Exception("User not found.");
            }

            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.Email = request.Email;
            existingUser.Password = request.Password;
            existingUser.UserType = request.UserType;
            existingUser.IsActive = request.IsActive;
            existingUser.RegistrationDate = request.RegistrationDate;

            await _unitOfWork.Users.UpdateAsync(existingUser);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteUser(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.CompleteAsync();
        }
    }
}