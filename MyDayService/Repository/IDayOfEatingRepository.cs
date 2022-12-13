using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MyDayService.Dtos.Request;
using MyDayService.Entity;

namespace MyDayService.Repository
{
    public interface IDayOfEatingRepository
    {
        Task<Doe> GetByDateAsync(int userId, DateTime date);

        Task<UpdateResult> UpdateAsync(int userId, DateTime date, Doe doe);

        Task<int> RemoveIngredientFromDoeAsync(int productId);
    }
}