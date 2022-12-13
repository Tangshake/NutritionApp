using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductsCatalog.Entities;

namespace ProductsCatalog.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDatabaseAccess _dbContext;

        public ProductRepository(ProductDatabaseAccess dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            if(product is null) throw new System.ArgumentNullException(nameof(product));

            await _dbContext.AddAsync(product);
            var result = await _dbContext.SaveChangesAsync();

            return result;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _dbContext.Product.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            //LINQ to Entities
            return await _dbContext.Product.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductByNameAsync(string name)
        {
            return await _dbContext.Product.Where(p => p.Name.ToLower().Contains(name.ToLower())).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByIdAsync(List<int> ids)
        {
            var result = await _dbContext.Product.Where(p => ids.Contains(p.Id)).ToListAsync();

            return result;
        }

        public async Task<int> RemoveProductAsync(int id)
        {
            if(id < 0) throw new System.ArgumentException(id.ToString());

            var product = new Entities.Product() { Id = id};
            _dbContext.ChangeTracker.Clear();
            _dbContext.Product.Attach(product);
            _dbContext.Product.Remove(product);
            var result = await _dbContext.SaveChangesAsync();

            return result;
        }

        public async Task<int> UpdateProductAsync(Product product)
        {
            _dbContext.ChangeTracker.Clear();
            _dbContext.Update(product);
            var result = await _dbContext.SaveChangesAsync();

            return result;
        }
    }
}