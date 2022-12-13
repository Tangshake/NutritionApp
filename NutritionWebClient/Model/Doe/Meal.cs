using System;
using System.Collections.Generic;


namespace NutritionWebClient.Model.Doe
{
    public class Meal
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public List<Ingredients> Ingredients { get; set; }
    }
}