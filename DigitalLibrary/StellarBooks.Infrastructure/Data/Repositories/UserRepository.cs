using Microsoft.EntityFrameworkCore;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Data;
using StellarBooks.Infrastructure.Data.Repositories;

namespace StellarBooks.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        private readonly StellarBocksApplicationDbContext _context;

        public UserRepository(StellarBocksApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersWithFavoritesAndTales()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .Include(u => u.Favorites)
                    .ThenInclude(f => f.Tale)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithFavoritesAndTalesById(int id)
        {
            return await _context.Users
                .Include(u => u.Favorites)
                    .ThenInclude(f => f.Tale)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }
    }
}