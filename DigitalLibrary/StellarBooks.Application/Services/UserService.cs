using AutoMapper;
using StellarBooks.Application.Interfaces;
using StellarBooks.Applications.DTOs;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UpdateUserDto>> GetUsers()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var usersResponse = _mapper.Map<List<UpdateUserDto>>(users);

            return usersResponse;
        }

        public async Task<List<UpdateUserDto>> GetAllUsersWithFavorites()
        {
            var users = await _unitOfWork.Users.GetAllUsersWithFavorites();
            var usersResponse = _mapper.Map<List<UpdateUserDto>>(users);

            return usersResponse;
        }

        public async Task<UpdateUserDto> GetUserById(int id)
        {
            var user = await _unitOfWork.Users.GetUserWithFavoritesById(id);

            if (user == null)
            {
                throw new Exception($"User with ID {id} not found.");
            }

            var userResponse = _mapper.Map<UpdateUserDto>(user);

            return userResponse;
        }

        public async Task<int> CreateUser(CreateUserDto request)
        {
            if (request == null)
            {
                throw new Exception("User cannot be null.");
            }

            var user = _mapper.Map<User>(request);
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

            _mapper.Map(request, existingUser);

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