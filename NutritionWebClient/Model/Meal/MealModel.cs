using System;
using System.Collections.Generic;
using NutritionWebClient.Model.Product;

namespace NutritionWebClient.Model.Meal
{
    public class MealModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public List<ProductModel> Ingredients {get;set;} = new List<ProductModel>();

    }
}