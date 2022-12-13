using Microsoft.EntityFrameworkCore;
using ProductsCatalog.Entities;

namespace ProductsCatalog.Repositories
{
    public class ProductDatabaseAccess : DbContext
    {
        public ProductDatabaseAccess(DbContextOptions<ProductDatabaseAccess> opt) : base(opt)
        {
            
        }

        public DbSet<Product> Product { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}