using System;
using System.Collections.Generic;
using PredefinedMeals.Dtos.Common;

namespace PredefinedMeals.Dtos.Request
{
    public class MealUpdateRequestDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public List<IngredientDto> Ingredients { get; set; }
    }
}