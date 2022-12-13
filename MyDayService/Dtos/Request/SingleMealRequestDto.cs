using System;

namespace MyDayService.Dtos.Request
{
    public class SingleMealRequestDto
    {
        public Guid Id { get; set; }
        public int Weight { get; set; }
    }
}