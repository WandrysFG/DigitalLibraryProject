using StellarBooks.Domain.Entities;

namespace StellarBooks.Infrastructure.Interface
{
    public interface IFavoriteRepository : IRepository<Favorite>
    {
        Task<List<Favorite>> GetAllWithUserAndTale();
        Task<Favorite> GetByIdWithUserAndTale(int id);
    }
}