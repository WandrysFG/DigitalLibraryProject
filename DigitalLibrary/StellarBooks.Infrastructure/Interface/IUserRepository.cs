using StellarBooks.Domain.Entities;

namespace StellarBooks.Infrastructure.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<User>> GetAllUsersWithFavorites();
        Task<User?> GetUserWithFavoritesById(int id);
    }
}