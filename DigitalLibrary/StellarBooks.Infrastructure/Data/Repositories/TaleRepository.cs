using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Data;
using StellarBooks.Infrastructure.Data.Repositories;

namespace StellarBooks.Infrastructure.Repositories
{
    public class TaleRepository : GenericRepository<Tale>
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
    }
}