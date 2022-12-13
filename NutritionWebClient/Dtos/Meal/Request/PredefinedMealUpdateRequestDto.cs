using System;
using System.Collections.Generic;

namespace NutritionWebClient.Dtos.Meal.Request
{
    public class PredefinedMealUpdateRequestDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public List<IngredientRequestDto> Ingredients {get;set;} = new List<IngredientRequestDto>();

    }
}