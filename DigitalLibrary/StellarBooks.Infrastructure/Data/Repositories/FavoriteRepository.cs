using Microsoft.EntityFrameworkCore;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Data;
using StellarBooks.Infrastructure.Data.Repositories;

namespace StellarBooks.Infrastructure.Repositories
{
    public class FavoriteRepository : GenericRepository<Favorite>
    {
        private readonly StellarBocksApplicationDbContext _context;

        public FavoriteRepository(StellarBocksApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Favorite>> GetAllWithUserAndTale()
        {
            return await _context.Favorites
                .Include(f => f.User)
                .Include(f => f.Tale)
                .ToListAsync();
        }

        public async Task<Favorite> GetByIdWithUserAndTale(int id)
        {
            return await _context.Favorites
                .Include(f => f.User)
                .Include(f => f.Tale)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

    }
}
