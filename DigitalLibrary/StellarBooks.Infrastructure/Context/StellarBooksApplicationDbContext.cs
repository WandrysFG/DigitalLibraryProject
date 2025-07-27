using Microsoft.EntityFrameworkCore;
using StellarBooks.Domain.Entities;

namespace StellarBooks.Infrastructure.Context
{
    public class StellarBocksApplicationDbContext : DbContext
    {
        public StellarBocksApplicationDbContext(DbContextOptions<StellarBocksApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Tale> Tales { get; set; }
        public DbSet<Activity> Activities { get; set; }
    }
}