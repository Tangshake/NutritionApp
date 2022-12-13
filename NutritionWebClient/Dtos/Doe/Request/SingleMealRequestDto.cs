using System;

namespace NutritionWebClient.Dtos.Doe.Request
{
    public class SingleMealRequestDto
    {
        public Guid Id { get; set; }
        public int Weight { get; set; }
    }
}