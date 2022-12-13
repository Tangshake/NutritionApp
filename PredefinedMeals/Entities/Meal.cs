using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;

namespace PredefinedMeals.Entities
{
    public class Meal
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public List<Ingredient> Ingredients { get; set; }
    }
}