using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using PredefinedMeals.Dtos.RabbitMQ.ProductPublished;
using PredefinedMeals.Entities;

namespace PredefinedMeals.Repositories
{

    public class MealsRepository : IMealsRepository
    {
        private const string collectionName = "meals";
        private readonly IMongoCollection<Meal> dbCollection;

        private readonly FilterDefinitionBuilder<Meal> filterBuilder = Builders<Meal>.Filter;

        public MealsRepository(IMongoDatabase database)
        {
            // var mongoClient = new MongoClient("mongodb://localhost:27017");
            // var database = mongoClient.GetDatabase("PredefinedMeals");
            dbCollection = database.GetCollection<Meal>(collectionName);
        }

        public async Task<IReadOnlyCollection<Meal>> GetAllAsync(int userId)
        {
            FilterDefinition<Meal> filter = filterBuilder.Eq(entity => entity.UserId, userId);

            return await dbCollection.Find(filter).ToListAsync();
        }

        public async Task<Meal> GetByIdAsync(Guid id, int userId)
        {
            FilterDefinition<Meal> filter = filterBuilder.Eq(entity => entity.Id, id) & filterBuilder.Eq(entity => entity.UserId, userId);

            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }
        
        public async Task<List<Meal>> GetByNameAsync(string name, int userId)
        {
            FilterDefinition<Meal> filter = filterBuilder.Regex("Name", new BsonRegularExpression(name, "i"));

            var result = await dbCollection.Find(filter).ToListAsync();

            return result;
        }

        public async Task<Guid> CreateAsync(Meal entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            await dbCollection.InsertOneAsync(entity);

            return entity.Id;
        }

        public async Task<int> UpdateAsync(Meal entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            FilterDefinition<Meal> filter = filterBuilder.Eq(existing => existing.Id, entity.Id);
            var result = await dbCollection.ReplaceOneAsync(filter, entity);

            return (int)result.ModifiedCount;
        }

        public async Task<int> RemoveAsync(Guid id, int userId)
        {
            FilterDefinition<Meal> filter = filterBuilder.Eq(entity => entity.Id, id);
            var result = await dbCollection.DeleteOneAsync(filter);
            return (int)result.DeletedCount;
        }

        public async Task<int> UpdateIngredientAsync(ProductPublishedDto productPublishedDto)
        {
            if(productPublishedDto is null) throw new ArgumentNullException(nameof(productPublishedDto));

            var filter = Builders<Meal>.Filter.And(Builders<Meal>.Filter.ElemMatch(x=>x.Ingredients, p=>p.Id == productPublishedDto.Id));

            var update = Builders<Meal>.Update
                                .Set(c=>c.Ingredients[-1].Id, productPublishedDto.Id)
                                .Set(c=>c.Ingredients[-1].Name, productPublishedDto.Name)
                                .Set(c=>c.Ingredients[-1].Manufacturer, productPublishedDto.Manufacturer)
                                .Set(c=>c.Ingredients[-1].Kcal, productPublishedDto.Id)
                                .Set(c=>c.Ingredients[-1].Protein, productPublishedDto.Id)
                                .Set(c=>c.Ingredients[-1].Fat, productPublishedDto.Id)
                                .Set(c=>c.Ingredients[-1].Carbohydrates, productPublishedDto.Id)
                                .Set(c=>c.Ingredients[-1].Roughage, productPublishedDto.Id)
                                .Set(c=>c.Ingredients[-1].Roughage, productPublishedDto.Id);

            var result = await dbCollection.UpdateManyAsync(filter, update);

            return (int)result.ModifiedCount;
        }

        public async Task<List<Meal>> GetManyByIdAsync(List<string> ids)
        {
            var guids = ids.Select(x=> Guid.Parse(x)).ToList();

            var filter = new BsonDocument("_id", new BsonDocument("$in", new BsonArray(guids)));

            List<Meal> documents = await dbCollection.Find(Builders<Meal>.Filter.In(m => m.Id, guids)).ToListAsync();

            return documents;
        }

        public async Task<int> RemoveIngredientFromMeals(int productId)
        {
            var filter = Builders<Meal>.Filter.Where(m => m.Ingredients.Any(c => c.Id == productId));
            var update = Builders<Meal>.Update.PullFilter(m => m.Ingredients, m => m.Id == productId);
            
            var result = await dbCollection.UpdateManyAsync(filter, update);

            return (int)result.ModifiedCount;
        }
    }
}