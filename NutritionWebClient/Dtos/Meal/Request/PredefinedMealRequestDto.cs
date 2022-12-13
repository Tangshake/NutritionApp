using System.Collections.Generic;

namespace NutritionWebClient.Dtos.Meal.Request
{
    public class PredefinedMealRequestDto
    {
        public string Name { get; set; }

        public int UserId { get; set; }

        public List<IngredientRequestDto> Ingredients {get;set;} = new List<IngredientRequestDto>();

    }
}