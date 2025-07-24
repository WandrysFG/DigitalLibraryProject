using StellarBooks.Domain.Entities;

namespace StellarBooks.Infrastructure.Interface
{
    public interface IActivityRepository : IRepository<Activity>
    {
        List<Activity> GetActivitiesByTaleId(int taleId);
    }
}