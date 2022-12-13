using System.Collections.Generic;

namespace NutritionWebClient.Dtos.Doe.Response
{
    public class SingleEntryDto
    {
        public string Hour { get; set; }

        public List<SingleProductDto> Products { get; set; }

        public List<SingleMealDto> Meals { get; set; }
    }
}