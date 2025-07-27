using StellarBooks.Applications.DTOs;

namespace StellarBooks.Application.Interfaces
{
    public interface IUserService
    {
        Task<int> CreateUser(CreateUserDto request);
        Task DeleteUser(int id);
        Task<UpdateUserDto> GetUserById(int id);
        Task<List<UpdateUserDto>> GetUsers();
        Task<List<UpdateUserDto>> GetAllUsersWithFavorites();
        Task UpdateUser(int id, UpdateUserDto request);
    }
}