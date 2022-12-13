using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PredefinedMeals.Dtos.RabbitMQ.ProductPublished;
using PredefinedMeals.Entities;

namespace PredefinedMeals.Repositories
{
    public interface IMealsRepository
    {
        Task<Guid> CreateAsync(Meal entity);
        Task<IReadOnlyCollection<Meal>> GetAllAsync(int userId);
        Task<Meal> GetByIdAsync(Guid id, int userId);
        Task<List<Meal>> GetByNameAsync(string name, int userId);
        Task<int> RemoveAsync(Guid id, int userId);
        Task<int> UpdateAsync(Meal entity);
        Task<int> UpdateIngredientAsync(ProductPublishedDto productPublishedDto);
        Task<List<Meal>> GetManyByIdAsync(List<string> ids);

        //RabbitMQ Event
        Task<int> RemoveIngredientFromMeals(int productId);
    }
}