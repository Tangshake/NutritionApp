using System.Collections.Generic;
using System.Threading.Tasks;
using NutritionWebClient.Dtos.Products;
using NutritionWebClient.Dtos.Products.Request;

namespace NutritionWebClient.SyncDataService.Products
{
    public interface IProductDataClient
    {
        Task<ProductReadDto> GetProductByIdAsync(int userId, int id);

        Task<List<ProductReadDto>> GetProductsByNameAsync(int userId, string name);

        Task<bool> AddProductAsync(int userId, ProductCreateRequestDto product);
    }
}