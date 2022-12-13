using System;

namespace PredefinedMeals.Dtos.Request
{
    public class MealReadRequestDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
    }
}