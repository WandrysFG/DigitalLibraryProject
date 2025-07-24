namespace StellarBooks.Infrastructure.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> query);
        Task<T> GetAsync(Func<IQueryable<T>, IQueryable<T>> query);
        Task<T> GetByIdAsync(int id);
        Task UpdateAsync(T entity);
    }
}