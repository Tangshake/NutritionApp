using MongoDB.Bson.Serialization.Attributes;

namespace MyDayService.Entity
{
    [BsonNoId]
    public class SingleProduct
    {
        [BsonElement("Id")]
        public int Id { get; set; }
        public int Weight { get; set; }
    }
}