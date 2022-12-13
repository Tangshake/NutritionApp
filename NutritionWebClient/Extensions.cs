using System;
using System.Collections.Generic;
using System.Linq;
using LogService;
using NutritionWebClient.Dtos;
using NutritionWebClient.Dtos.Doe.Request;
using NutritionWebClient.Dtos.Doe.Response;
using NutritionWebClient.Dtos.Meal;
using NutritionWebClient.Dtos.Meal.Request;
using NutritionWebClient.Dtos.Meal.Response;
using NutritionWebClient.Dtos.Products;
using NutritionWebClient.Dtos.Products.Request;
using NutritionWebClient.Dtos.User;
using NutritionWebClient.Model;
using NutritionWebClient.Model.Log;
using NutritionWebClient.Model.Meal;
using NutritionWebClient.Model.Product;

namespace NutritionWebClient
{
    public static class Extensions
    {
        public static UserRegisterDto AsDto(this RegisterModel registerModel)
        {
            return new UserRegisterDto()
            {
                Login = registerModel.Login,
                Email = registerModel.Email,
                Password = registerModel.Password
            };
        }

        public static UserLoginDto AsDto(this LoginModel loginModel) 
        {
            return new UserLoginDto()
            {
                Email = loginModel.Email,
                Password = loginModel.Password
            };
        }

        public static IngredientRequestDto AsMealDto(this ProductReadDto productReadDto)
        {
            return new IngredientRequestDto()
            {
                Id = productReadDto.Id,
                Name = productReadDto.Name,
                Manufacturer = productReadDto.Manufacturer,
                Kcal = productReadDto.Kcal,
                Carbohydrates = productReadDto.Carbohydrates,
                Fat = productReadDto.Fat,
                Protein = productReadDto.Protein,
                Roughage = productReadDto.Roughage
            };
        }

        public static ProductModel AsProductModel(this ProductReadDto productReadDto)
        {
            return new ProductModel()
            {
                Id = productReadDto.Id,
                Name = productReadDto.Name,
                Manufacturer = productReadDto.Manufacturer,
                Kcal = productReadDto.Kcal,
                Carbohydrates = productReadDto.Carbohydrates,
                Fat = productReadDto.Fat,
                Protein = productReadDto.Protein,
                Roughage = productReadDto.Roughage,
                Weight = 100
            };
        }

        public static ProductModel AsProductModel(this IngredientResponseDto ingredientResponseDto)
        {
            return new ProductModel()
            {
                Id = ingredientResponseDto.Id,
                Name = ingredientResponseDto.Name,
                Manufacturer = ingredientResponseDto.Manufacturer,
                Kcal = ingredientResponseDto.Kcal,
                Carbohydrates = ingredientResponseDto.Carbohydrates,
                Fat = ingredientResponseDto.Fat,
                Protein = ingredientResponseDto.Protein,
                Roughage = ingredientResponseDto.Roughage,
                Weight = ingredientResponseDto.Weight
            };
        }

        public static MealModel AsMealModel(this PredefinedMealResponseDto predefinedMealResponseDto)
        {
            var ingredients = predefinedMealResponseDto.Ingredients.Select(x=>x.AsProductModel()).ToList();

            return new MealModel()
            {
                Id = predefinedMealResponseDto.Id,
                Name = predefinedMealResponseDto.Name,
                UserId = predefinedMealResponseDto.UserId,
                Ingredients = new List<ProductModel>(ingredients)
            };
        }

        public static IngredientRequestDto AsProductDto(this ProductModel product)
        {
            return new IngredientRequestDto()
            {
                Id = product.Id,
                Name = product.Name,
                Manufacturer = product.Manufacturer,
                Kcal = product.Kcal,
                Carbohydrates = product.Carbohydrates,
                Fat = product.Fat,
                Protein = product.Protein,
                Roughage = product.Roughage,
                Weight = product.Weight
            };
        }

        public static ProductCreateRequestDto AsProductCreateRequestDto(this ProductModel product)
        {
            return new ProductCreateRequestDto()
            {
                Name = product.Name,
                Manufacturer = product.Manufacturer??"N/A",
                Kcal = product.Kcal,
                Carbohydrates = product.Carbohydrates,
                Fat = product.Fat,
                Protein = product.Protein,
                Roughage = product.Roughage,
            };
        }

        public static PredefinedMealUpdateRequestDto AsMealRequestDto(this MealModel meal)
        {
            var ingredients = meal.Ingredients.Select(x=>x.AsProductDto()).ToList();

            return new PredefinedMealUpdateRequestDto()
            {
                Id = meal.Id,
                Name = meal.Name,
                UserId = meal.UserId,
                Ingredients = new List<IngredientRequestDto>(ingredients)
            };
        }

        public static DoeRequestDto AsDoeRequestDto(this DoeResponseDto doeResponseDto)
        {
            var doeRequestDto = new DoeRequestDto();
            doeRequestDto.Date = doeResponseDto.Date.ToUniversalTime().Date;
            doeRequestDto.Id = doeResponseDto.Id;
            doeRequestDto.UserId = doeResponseDto.UserId;
            doeRequestDto.Does = new List<SingleEntryRequestDto>();
            Console.WriteLine($"Number of does: {doeResponseDto.Does.Count}");
            Console.WriteLine($"Date: {doeResponseDto.Date} : {doeRequestDto.Date}");

            foreach(var doe in doeResponseDto.Does)
            {
                Console.WriteLine($"Number of meals: {doe.Meals.Count}");
                var singleEntryRequestDto = new SingleEntryRequestDto();
                singleEntryRequestDto.Hour = doe.Hour;
                singleEntryRequestDto.Meals = new List<SingleMealRequestDto>();
                singleEntryRequestDto.Products = new List<SingleProductRequestDto>();

                foreach(var meal in doe.Meals)
                {
                    if(meal.Meal is not null) 
                    {
                        var singleMealRequestDto = new SingleMealRequestDto();
                        singleMealRequestDto.Id = meal.Meal.Id;
                        singleMealRequestDto.Weight = meal.Weight;

                        singleEntryRequestDto.Meals.Add(singleMealRequestDto);
                    }
                }

                foreach(var product in doe.Products)
                {
                    if(product.Product is not null)
                    {
                        var singleProductRequestDto = new SingleProductRequestDto();
                        singleProductRequestDto.Id = product.Product.Id;
                        singleProductRequestDto.Weight = product.Weight;

                        singleEntryRequestDto.Products.Add(singleProductRequestDto);
                    }
                }

                doeRequestDto.Does.Add(singleEntryRequestDto);
            }

            Console.WriteLine($"[AsDoeRequestDto] Number of does: {doeRequestDto.Does.Count}");
            return doeRequestDto;
        }

        public static List<Log> AsLogModel(this GrpcResponseLogsDto grpcResponseLogDto)
        {
            
            var logs = new List<Log>();

            foreach(var log in grpcResponseLogDto.Logs)
            {
                logs.Add(new Log() {
                   Id = log.Id,
                   Date = log.Date.ToDateTime(),
                   UserId = log.UserId,
                   Message = log.Message,
                   Method = log.Method,
                   Error = log.Error,
                   ServiceName = log.ServiceName 
                });
            }

            return logs;
        }
    }
}