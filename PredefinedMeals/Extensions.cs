using System.Linq;
using PredefinedMeals.Dtos.Response;
using PredefinedMeals.Dtos.Request;
using PredefinedMeals.Entities;
using PredefinedMeals.Dtos.Common;
using MealsCatalog;

namespace PredefinedMeals
{
    public static class Extensions
    {
        public static MealResponseDto AsReadDto(this Meal meal)
        {
            var item = new MealResponseDto()
            {
                Id = meal.Id,
                Name = meal.Name,
                UserId = meal.UserId,
                Ingredients = new System.Collections.Generic.List<IngredientDto>()
            };

            var result = meal.Ingredients.Select(x=>x.AsDto());
            item.Ingredients.AddRange(result);

            return item;
        }

        public static IngredientDto AsDto(this Ingredient ingredient)
        {
            return new IngredientDto()
            {
                Id = ingredient.Id,
                Carbohydrates = ingredient.Carbohydrates,
                Fat = ingredient.Fat,
                Kcal = ingredient.Kcal,
                Manufacturer = ingredient.Manufacturer,
                Name = ingredient.Name,
                Protein = ingredient.Protein,
                Roughage = ingredient.Roughage,
                Weight = ingredient.Weight,
            };
        }

        public static Ingredient AsEntity(this IngredientDto ingredient)
        {
            return new Ingredient()
            {
                Id = ingredient.Id,
                Carbohydrates = ingredient.Carbohydrates,
                Fat = ingredient.Fat,
                Kcal = ingredient.Kcal,
                Manufacturer = ingredient.Manufacturer,
                Name = ingredient.Name,
                Protein = ingredient.Protein,
                Roughage = ingredient.Roughage,
                Weight = ingredient.Weight,
            };
        }

        public static MealModel AsGrpcMealModel(this Meal meal)
        {
            var mealModel = new MealModel();
            mealModel.MealId = meal.Id.ToString();
            mealModel.Name = meal.Name;
            mealModel.UserId = meal.UserId;

            foreach(var item in meal.Ingredients)
            {
                var ingredient = new IngredientModel();
                ingredient.IngredientId = item.Id;
                ingredient.Carbohydrates = item.Carbohydrates;
                ingredient.Fat = item.Fat;
                ingredient.Kcal = item.Kcal;
                ingredient.Manufacturer = item.Manufacturer;
                ingredient.Name = item.Name;
                ingredient.Protein = item.Protein;
                ingredient.Roughage = item.Roughage;
                ingredient.Weight = item.Weight;

                mealModel.Ingredients.Add(ingredient);
            }

            return mealModel;
        }
    }
}