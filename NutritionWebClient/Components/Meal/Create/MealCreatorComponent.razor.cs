using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NutritionWebClient.Dtos.Meal.Request;
using NutritionWebClient.Dtos.Products;
using NutritionWebClient.Services.InformationDialog;
using NutritionWebClient.SyncDataService.Meals;
using NutritionWebClient.SyncDataService.Products;

namespace NutritionWebClient.Components.Meal.Create
{

    public partial class MealCreatorComponent : ComponentBase
    {
        [Inject]
        public IProductDataClient _productRepository {get;set;}

        [Inject]
        public IMealDataClient _mealRepository { get; set; }
        
        [Inject]
        public InformationDialogService _informationDialogService { get; set; }
        
        [Parameter]
        public int UserId { get; set; }

        private ShowInfo Information = ShowInfo.NoProduct;

        private ProductReadDto Product { get; set; }

        private List<ProductReadDto> products;

        private PredefinedMealRequestDto PredefinedMeal { get; set; }

        private MealSummary Summary = new MealSummary();

        private async Task AddUserDefinedMeal()
        {
            Console.WriteLine($"[AddUserDefinedMeal] Meal name is: {PredefinedMeal.Name}");
            Console.WriteLine($"[AddUserDefinedMeal] Meal added by user: {PredefinedMeal.UserId}");
            Console.WriteLine($"[AddUserDefinedMeal] Meal contains {PredefinedMeal.Ingredients.Count} ingredient");
            var result = await _mealRepository.CreateMealAsync(PredefinedMeal);
            Console.WriteLine($"[AddUserDefinedMeal] Result: {result}");

            if(!string.IsNullOrEmpty(result))
            {
                _informationDialogService.ShowInformationDialog("Twój posiłek został utworzony!.", DialogType.Success);

                Information = ShowInfo.MealSuccess;
                PredefinedMeal = null;
                Summary = new MealSummary();
                Product = null;
                products.Clear();
            }
            else
            {
                Information = ShowInfo.MealFail;
                _informationDialogService.ShowInformationDialog("Ups! Coś poszło nie tak.", DialogType.Error);
            }
        }

        public async void OnSearchStringChange(string search)
        {
            Console.WriteLine($"Search string: {search}");
            Information = ShowInfo.NoProduct;

            if(!string.IsNullOrEmpty(search) && search.Length > 2)
                await SearchForProducts(search);
        }

        public void ProductSelectedEvent(int index)
        {
            Product = products[index];
        }

        public async Task SearchForProducts(string search)
        {
            products = await _productRepository.GetProductsByNameAsync(UserId, search);

            StateHasChanged();
        }

        private void AddProductToMeal()
        {
            if(PredefinedMeal is null) PredefinedMeal = new PredefinedMealRequestDto();

            var mealProduct = Product.AsMealDto();
            mealProduct.Weight = 100;
            PredefinedMeal.UserId = UserId;
            PredefinedMeal.Name = "Nowa nazwa";
            PredefinedMeal.Ingredients.Add(mealProduct);
            
            CalculateMealSummary();
        }

        private void CalculateMealSummary()
        {
            if(Summary is null) Summary = new MealSummary();
            
            Summary.ClearAll();
            foreach(var product in PredefinedMeal.Ingredients)
            {
                Summary.Kcal += (product.Kcal * product.Weight) / 100;
                Summary.Weight += product.Weight;
                Summary.Protein += (product.Protein * product.Weight) / 100;
                Summary.Carbohydrates += (product.Carbohydrates * product.Weight) / 100;
                Summary.Fat += (product.Fat * product.Weight) / 100;
                Summary.Roughage += (product.Roughage * product.Weight) / 100;
            }
        }

        private void OnProductRemoveEvent(int index)
        {
            PredefinedMeal.Ingredients.RemoveAt(index);
            CalculateMealSummary();
        }

        private void OnWeightChange(ChangeEventArgs args, int index)
        {
            Console.WriteLine($"[OnWeightChange] {args.Value.ToString()}");
            var weight = float.Parse(args.Value.ToString());

            if(weight <= 1)
            {
                PredefinedMeal.Ingredients[index].Weight = 1;
                StateHasChanged();
            }
            else if(weight >= 10000)
            {
                PredefinedMeal.Ingredients[index].Weight = 10000;
                StateHasChanged();
            }
            else
            {
                PredefinedMeal.Ingredients[index].Weight = float.Parse(args.Value.ToString());
                CalculateMealSummary();
                StateHasChanged();
            }
        }
    }

    public class MealSummary
    {
        public float Weight { get; set; }
        public float Kcal { get; set; }
        public float Protein { get; set; }
        public float Carbohydrates { get; set; }
        public float Fat { get; set; }
        public float Roughage { get; set; }

        public void ClearAll()
        {
            Weight = 0;
            Kcal = 0;
            Protein = 0;
            Carbohydrates = 0;
            Fat = 0;
            Roughage = 0;
        }
    }

    public enum ShowInfo
    {
        MealSuccess,
        MealFail,
        NoProduct
    }
}