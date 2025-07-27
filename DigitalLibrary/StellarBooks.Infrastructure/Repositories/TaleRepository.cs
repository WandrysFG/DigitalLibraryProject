using Microsoft.EntityFrameworkCore;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Context;
using StellarBooks.Infrastructure.Core;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Infrastructure.Repositories
{
    public class TaleRepository : GenericRepository<Tale>, ITaleRepository
    {
        private readonly StellarBocksApplicationDbContext _context;

        public TaleRepository(StellarBocksApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public List<Tale> GetTalesByTitle(string title)
        {
            return _context.Tales
                .Where(t => t.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<Tale> GetTaleWithRelations(int id)
        {
            return await _context.Tales
                .Include(t => t.Activities)
                .Include(t => t.Favorites)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}