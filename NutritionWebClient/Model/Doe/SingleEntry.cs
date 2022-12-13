using System.Collections.Generic;

namespace NutritionWebClient.Model.Doe
{
    public class SingleEntry
    {
        public string Hour { get; set; }

        public List<SingleProduct> Products { get; set; }

        public List<SingleMeal> Meals { get; set; }
    }
}