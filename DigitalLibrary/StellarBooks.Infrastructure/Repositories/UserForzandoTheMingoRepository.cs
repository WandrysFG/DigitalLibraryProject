using Microsoft.EntityFrameworkCore;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Context;
using StellarBooks.Infrastructure.Core;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Infrastructure.Repositories
{
    public class UserForzandoTheMingoRepository : GenericRepository<User>, IUserRepository
    {
        private readonly StellarBocksApplicationDbContext _context;

        public UserForzandoTheMingoRepository(StellarBocksApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersWithFavorites()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .Include(u => u.Favorites)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithFavoritesById(int id)
        {
            Console.WriteLine($"Fetching users for favorite ID: {id}");
            return await _context.Users
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }
    }
}