namespace StellarBooks.Infrastructure.Interface
{
    public interface IUnitOfWork
    {
        IActivityRepository Activities { get; }
        IFavoriteRepository Favorites { get; }
        ITaleRepository Tales { get; }
        IUserRepository Users { get; }

        Task CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}