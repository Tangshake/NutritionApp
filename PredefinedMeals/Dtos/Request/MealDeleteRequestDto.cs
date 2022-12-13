using System;

namespace PredefinedMeals.Dtos.Request
{
    public class MealDeleteRequestDto
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
    }
}