using System;
using System.Collections.Generic;
using NutritionWebClient.Dtos.Products;

namespace NutritionWebClient.Dtos.Meal.Response
{
    public class PredefinedMealResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public List<IngredientResponseDto> Ingredients {get;set;} = new List<IngredientResponseDto>();

    }
}