using System;

namespace MyDayService.Dtos.Response
{
    public class SingleMealDto
    {
        public MealResponseDto Meal { get; set; }
        public int Weight { get; set; }
    }
}