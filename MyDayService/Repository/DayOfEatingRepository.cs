using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MyDayService.Dtos.Request;
using MyDayService.Entity;

namespace MyDayService.Repository
{
    public class DayOfEatingRepository : IDayOfEatingRepository
    {
        private const string collectionName = "does";
        private readonly IMongoCollection<Doe> _dbCollection;
        private readonly FilterDefinitionBuilder<Doe> filterBuilder = Builders<Doe>.Filter;

        public DayOfEatingRepository(IMongoDatabase database, IConfiguration configuration)
        {
            _dbCollection = database.GetCollection<Doe>(collectionName);
        }

        public async Task<Doe> GetByDateAsync(int userId, DateTime date)
        {
            Console.WriteLine("[DayOfEatingService]");
            Console.WriteLine($"[GetByDateAsync] User: {userId} requested his doe from date: {date}");
            FilterDefinition<Doe> filter = filterBuilder.Eq(entity => entity.UserId, userId) & filterBuilder.Eq(entity => entity.Date, date);
            
            var result = await _dbCollection.Find(filter).SingleOrDefaultAsync();
            Console.WriteLine($"[GetByDateAsync] Got results from database.");

            return result;
        }

        public async Task<int> RemoveIngredientFromDoeAsync(int productId)
        {
            Console.WriteLine("[RemoveIngredientFromDoeAsync]");
            var filter = Builders<Doe>.Filter.Eq("Does.Products.Id", productId);
            var update = Builders<Doe>.Update.PullFilter("Does.$.Products", Builders<SingleProduct>.Filter.Eq(d => d.Id, productId));
            
            var result = await _dbCollection.UpdateManyAsync(filter, update);
            Console.WriteLine($"[RemoveIngredientFromDoeAsync] Result: {result.MatchedCount}");

            return (int)result.ModifiedCount;
        }

        public async Task<UpdateResult> UpdateAsync(int userId, DateTime date, Doe doe)
        {
            Console.WriteLine($"[UpdateAsync] User: {userId} requested to update or create his his doe.");
            Console.WriteLine($"[UpdateAsync] Number of does: {doe.Does.Count} DoeDate: {doe.Date} Date: {date} UniversalDate: {date.ToUniversalTime()}");
            var result = await _dbCollection.UpdateOneAsync(
                Builders<Doe>.Filter.Eq(entity => entity.UserId, userId) & filterBuilder.Eq(entity => entity.Date, doe.Date),
                Builders<Doe>.Update
                    .Set(x=>x.Date, doe.Date)
                    .Set(x=>x.Does, doe.Does)
                    .Set(x=>x.UserId, doe.UserId)
                    .SetOnInsert(x => x.Id, Guid.NewGuid()),
                    new UpdateOptions{IsUpsert = true}
            );

            return result;
        }
    }
}