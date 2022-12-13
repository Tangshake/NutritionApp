using System.Collections.Generic;
using System.Threading.Tasks;
using ProductsCatalog.Entities;

namespace ProductsCatalog.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductByNameAsync(string name);
        Task<int> CreateProductAsync(Product product);
        Task<int> RemoveProductAsync(int id);
        Task<int> UpdateProductAsync(Product product);
        Task<IEnumerable<Product>> GetProductsByIdAsync(List<int> ids);
    }
}