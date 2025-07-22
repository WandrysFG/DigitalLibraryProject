using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarBooks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace StellarBooks.Infrastructure.Data.Repositories
{
    public class GenericRepository<T> where T : class
    {
        private readonly StellarBocksApplicationDbContext _context;
        public GenericRepository(StellarBocksApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        
        public async Task<List<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> query)
        {
            return await query(_context.Set<T>()).ToListAsync();
        }

        public async Task<T> GetAsync(Func<IQueryable<T>, IQueryable<T>> query)
        {
            return await query(_context.Set<T>()).FirstOrDefaultAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
