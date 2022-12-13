using System;

namespace NutritionWebClient.Dtos.Doe.Response
{
    public class SingleMealDto
    {
        public MealResponseDto Meal { get; set; }
        public int Weight { get; set; }
    }
}