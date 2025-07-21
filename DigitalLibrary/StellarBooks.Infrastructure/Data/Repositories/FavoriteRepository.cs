using Microsoft.EntityFrameworkCore;
using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Data;

namespace StellarBooks.Infrastructure.Repositories
{
    public class FavoriteRepository
    {
        private readonly StellarBocksApplicationDbContext _context;

        public FavoriteRepository(StellarBocksApplicationDbContext context)
        {
            _context = context;
        }

        public List<Favorite> GetAllFavorites()
        {
            //return _context.Favorites.ToList();
            return _context.Favorites
            .Include(f => f.User)
            .Include(f => f.Tale)
            .ToList();
        }

        public Favorite GetFavoriteById(int id)
        {
            //return _context.Favorites.FirstOrDefault(f => f.Id == id);
            return _context.Favorites
            .Include(f => f.User)
            .Include(f => f.Tale)
            .FirstOrDefault(f => f.Id == id);
        }

        public List<Favorite> GetFavoritesByUserId(int userId)
        {
            return _context.Favorites
                .Where(f => f.UserId == userId)
                .ToList();
        }

        public int AddFavorite(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
            _context.SaveChanges();
            return favorite.Id;
        }

        public void UpdateFavorite(Favorite favorite)
        {
            _context.Favorites.Update(favorite);
            _context.SaveChanges();
        }

        public void DeleteFavorite(int id)
        {
            var favorite = GetFavoriteById(id);
            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                _context.SaveChanges();
            }
        }
    }
}
