using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
using PredefinedMeals.Dtos.Common;

namespace PredefinedMeals.Dtos.Response
{
    public class MealResponseDto
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public List<IngredientDto> Ingredients { get; set; }
    }

}