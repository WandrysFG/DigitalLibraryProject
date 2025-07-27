using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StellarBooks.Infrastructure.Context;
using StellarBooks.Infrastructure.Interface;

namespace StellarBooks.Infrastructure.Core
{
    public class GenericRepository<T> : IRepository<T> where T : class
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
            return entity;
        }
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
        }
        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
    }
}
