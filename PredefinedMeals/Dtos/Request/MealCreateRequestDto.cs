using System;
using System.Collections.Generic;
using PredefinedMeals.Dtos.Common;

namespace PredefinedMeals.Dtos.Request
{
    public class MealCreateRequestDto
    {
        public string Name { get; set; }

        public int UserId { get; set; }

        public List<IngredientDto> Ingredients { get; set; }
    }
}