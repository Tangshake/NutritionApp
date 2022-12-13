using System;
using System.Collections.Generic;

namespace NutritionWebClient.Dtos.Doe.Request
{
    public class SingleEntryRequestDto
    {
        public string Hour { get; set; }

        public List<SingleProductRequestDto> Products { get; set; }

        public List<SingleMealRequestDto> Meals { get; set; }
    }
}