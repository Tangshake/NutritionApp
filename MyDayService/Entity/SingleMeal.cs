using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyDayService.Entity
{
    [BsonNoId]
    public class SingleMeal
    {
        [BsonRepresentation(BsonType.String)]
        [BsonElement("Id")]
        public Guid Id { get; set; }
        public int Weight { get; set; }
    }
}