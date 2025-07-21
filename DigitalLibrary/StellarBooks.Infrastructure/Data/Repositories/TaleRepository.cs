using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Data;

namespace StellarBooks.Infrastructure.Repositories
{
    public class TaleRepository
    {
        private readonly StellarBocksApplicationDbContext _context;

        public TaleRepository(StellarBocksApplicationDbContext context)
        {
            _context = context;
        }

        public List<Tale> GetAllTales()
        {
            return _context.Tales.ToList();
        }

        public Tale GetTaleById(int id)
        {
            return _context.Tales.FirstOrDefault(t => t.Id == id);
        }

        public List<Tale> GetTalesByTitle(string title)
        {
            return _context.Tales
                .Where(t => t.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public int AddTale(Tale tale)
        {
            _context.Tales.Add(tale);
            _context.SaveChanges();
            return tale.Id;
        }

        public void UpdateTale(Tale tale)
        {
            _context.Tales.Update(tale);
            _context.SaveChanges();
        }

        public void DeleteTale(int id)
        {
            var tale = GetTaleById(id);
            if (tale != null)
            {
                _context.Tales.Remove(tale);
                _context.SaveChanges();
            }
        }
    }
}
