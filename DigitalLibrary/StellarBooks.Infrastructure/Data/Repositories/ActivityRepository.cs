using StellarBooks.Domain.Entities;
using StellarBooks.Infrastructure.Data;

namespace StellarBooks.Infrastructure.Repositories
{
    public class ActivityRepository
    {
        private readonly StellarBocksApplicationDbContext _context;

        public ActivityRepository(StellarBocksApplicationDbContext context)
        {
            _context = context;
        }

        public List<Activity> GetAllActivities()
        {
            return _context.Activities.ToList();
        }

        public Activity GetActivityById(int id)
        {
            return _context.Activities.FirstOrDefault(a => a.Id == id);
        }

        public List<Activity> GetActivitiesByTaleId(int taleId)
        {
            return _context.Activities
                .Where(a => a.TaleId == taleId)
                .ToList();
        }

        public int AddActivity(Activity activity)
        {
            _context.Activities.Add(activity);
            _context.SaveChanges();
            return activity.Id;
        }

        public void UpdateActivity(Activity activity)
        {
            _context.Activities.Update(activity);
            _context.SaveChanges();
        }

        public void DeleteActivity(int id)
        {
            var activity = GetActivityById(id);
            if (activity != null)
            {
                _context.Activities.Remove(activity);
                _context.SaveChanges();
            }
        }
    }
}