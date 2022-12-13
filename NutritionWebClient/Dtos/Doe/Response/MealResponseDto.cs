using System;
using System.Collections.Generic;


namespace NutritionWebClient.Dtos.Doe.Response
{
    public class MealResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public List<IngredientsResponseDto> Ingredients { get; set; }
    }
}