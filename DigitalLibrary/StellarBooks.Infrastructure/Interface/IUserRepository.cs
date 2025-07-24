using StellarBooks.Domain.Entities;

namespace StellarBooks.Infrastructure.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<User>> GetAllUsersWithFavoritesAndTales();
        Task<User?> GetUserWithFavoritesAndTalesById(int id);
    }
}