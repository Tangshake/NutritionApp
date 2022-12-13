using System;
using System.Collections.Generic;


namespace MyDayService.Dtos.Response
{
    public class MealResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public List<IngredientResponseDto> Ingredients { get; set; }
    }
}