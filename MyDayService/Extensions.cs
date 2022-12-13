using System;
using System.Collections.Generic;
using System.Linq;
using MealsCatalog;
using MyDayService.Dtos.Request;
using MyDayService.Dtos.Response;
using MyDayService.Entity;
using ProductsCatalog;

namespace MyDayService
{
    public static class Extensions
    {
        public static GrpcRequestProductsModel ExtractGrpcRequestProductModel(this Doe doe)
        {
            var listOfGrpcProductsModel = doe.Does.SelectMany(x => x.Products.Select(p => new ProductId(){ProductId_ = p.Id}));

            GrpcRequestProductsModel grpcRequestProductsModel = new ();
            grpcRequestProductsModel.Ids.AddRange(listOfGrpcProductsModel);

            return grpcRequestProductsModel;
        }

        public static GrpcRequestMealsModel ExtractGrpcRequestMealModel(this Doe doe)
        {
            var listOfGrpcMealsModel = doe.Does.SelectMany(x => x.Meals.Select(m => new MealsId(){MealId = m.Id.ToString()}));

            GrpcRequestMealsModel grpcRequestMealsModel = new ();
            grpcRequestMealsModel.Ids.AddRange(listOfGrpcMealsModel);

            return grpcRequestMealsModel;
        }

        public static ProductResponseDto AsProductResponseDto(this ProductModel productModel)
        {
            var productResponseDto = new ProductResponseDto();
            productResponseDto.Id = productModel.Id;
            productResponseDto.Name = productModel.Name;
            productResponseDto.Manufacturer = productModel.Manufacturer;
            productResponseDto.Kcal = productModel.Kcal;
            productResponseDto.Carbohydrates = productModel.Carbohydrates;
            productResponseDto.Fat = productModel.Fat;
            productResponseDto.Protein = productModel.Protein;
            productResponseDto.Roughage = productModel.Roughage;

            return productResponseDto;
        }

        public static MealResponseDto AsMealResponseDto(this MealModel mealModel)
        {
           var mealResponseDto = new MealResponseDto();
           mealResponseDto.Id = Guid.Parse(mealModel.MealId);
           mealResponseDto.Name = mealModel.Name;
           mealResponseDto.UserId = mealModel.UserId;
           mealResponseDto.Ingredients = new List<IngredientResponseDto>();

           foreach(var item in mealModel.Ingredients)
           {
                var ingredient = new IngredientResponseDto();
                ingredient.Id = item.IngredientId;
                ingredient.Name = item.Name;
                ingredient.Kcal = item.Kcal;
                ingredient.Manufacturer = item.Manufacturer;
                ingredient.Fat = item.Fat;
                ingredient.Protein = item.Protein;
                ingredient.Roughage = item.Roughage;
                ingredient.Weight = item.Weight;
                ingredient.Carbohydrates = item.Carbohydrates;
                
                mealResponseDto.Ingredients.Add(ingredient);
           }

           return mealResponseDto;
        }

        public static DoeResponseDto AsDoeResponseDto(this Doe doe, GrpcResponseProductModel grpcResponseProductModel, GrpcResponseMealsModel grpcResponseMealsModel)
        {
            var doeResponseDto = new DoeResponseDto();
            doeResponseDto.Id = doe.Id;
            doeResponseDto.Date = doe.Date;
            doeResponseDto.UserId = doe.UserId;
            doeResponseDto.Does = new List<SingleEntryDto>();

            foreach(var item in doe.Does)
            {
                var singleEntryDto = new SingleEntryDto();
                singleEntryDto.Meals = new List<SingleMealDto>();
                singleEntryDto.Products = new List<SingleProductDto>();

                singleEntryDto.Hour = item.Hour;
                
                foreach(var m in item.Meals)
                {
                    var singleMealDto = new SingleMealDto();
                    singleMealDto.Weight = m.Weight;
                    
                    var result = grpcResponseMealsModel.Meals.FirstOrDefault(o => o.MealId == m.Id.ToString());

                    if(result is not null)
                        singleMealDto.Meal = result.AsMealResponseDto();

                    singleEntryDto.Meals.Add(singleMealDto);
                }

                foreach(var p in item.Products)
                {
                    var singleProductDto = new SingleProductDto();
                    singleProductDto.Weight = p.Weight;
                    
                    if(grpcResponseProductModel is not null)
                    {
                        var result = grpcResponseProductModel.Products.FirstOrDefault(o => o.Id == p.Id);

                        if(result is not null)
                            singleProductDto.Product = result.AsProductResponseDto();

                        singleEntryDto.Products.Add(singleProductDto);
                    }
                }

                doeResponseDto.Does.Add(singleEntryDto);
            }

            return doeResponseDto;
        }
    }
}