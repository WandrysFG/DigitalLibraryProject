using StellarBooks.Domain.Entities;

namespace StellarBooks.Infrastructure.Interface
{
    public interface ITaleRepository : IRepository<Tale>
    {
        List<Tale> GetTalesByTitle(string title);
        Task<Tale> GetTaleWithRelations(int id);
    }
}