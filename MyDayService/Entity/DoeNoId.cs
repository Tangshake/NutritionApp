using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MyDayService.Entity
{
    public class DoeNoId
    {
        //[BsonId(IdGenerator = typeof(IdGenerator))]
        // [BsonRepresentation(BsonType.String)]
        // [BsonIgnoreIfDefault]
        // [BsonId]


        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

        public int UserId { get; set ;}

        public List<SingleEntry> Does { get; set; }
    }
}