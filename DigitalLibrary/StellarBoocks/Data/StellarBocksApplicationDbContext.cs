using Microsoft.EntityFrameworkCore;
using StellarBoocks.Entities;

namespace StellarBoocks.Data
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
