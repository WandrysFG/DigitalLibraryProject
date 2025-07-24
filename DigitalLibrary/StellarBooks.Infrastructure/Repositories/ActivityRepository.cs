using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Infrastructure.Repositories
{
    public class ActivityRepository : GenericRepository<Activity>, IActivityRepository
    {
        private readonly StellarBocksApplicationDbContext _context;

        public ActivityRepository(StellarBocksApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public List<Activity> GetActivitiesByTaleId(int taleId)
        {
            return _context.Activities
                .Where(a => a.TaleId == taleId)
                .ToList();
        }
    }
}