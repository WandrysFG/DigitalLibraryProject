using StellarBooks.Infrastructure.Context;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StellarBocksApplicationDbContext _context;
        private IUserRepository _userRepository;
        private ITaleRepository _taleRepository;
        private IFavoriteRepository _favoriteRepository;
        private IActivityRepository _activityRepository;

        public UnitOfWork(StellarBocksApplicationDbContext context, IUserRepository userRepository, ITaleRepository taleRepository, IFavoriteRepository favoriteRepository, IActivityRepository activityRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _taleRepository = taleRepository;
            _favoriteRepository = favoriteRepository;
            _activityRepository = activityRepository;
        }
        public IUserRepository Users => _userRepository;

        public ITaleRepository Tales => _taleRepository;

        public IFavoriteRepository Favorites => _favoriteRepository;

        public IActivityRepository Activities => _activityRepository;

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
}