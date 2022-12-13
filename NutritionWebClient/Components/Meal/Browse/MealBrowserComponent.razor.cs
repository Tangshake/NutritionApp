using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NutritionWebClient.Components.Meal.Browse.SingleMeal;
using NutritionWebClient.Dtos.Meal.Response;
using NutritionWebClient.Dtos.Products;
using NutritionWebClient.Model.Meal;
using NutritionWebClient.Model.Product;
using NutritionWebClient.Services.InformationDialog;
using NutritionWebClient.SyncDataService.Meals;
using NutritionWebClient.SyncDataService.Products;

namespace NutritionWebClient.Components.Meal.Browse
{
    public partial class MealBrowserComponent :ComponentBase
    {
        [Inject]
        public IMealDataClient _mealRepository { get; set; }

        [Inject]
        public IProductDataClient _productRepository { get; set; }

        [Inject]
        public InformationDialogService _informationDialogService { get; set; }

        [Parameter]
        public int UserId { get; set; }

        MealDetails MealDetailsComponentReference;
        
        private bool ShowSearchButton { get; set; } = true;
        public string SearchText { get; set; }

        private bool editMode = false;
        private bool HideMealDetailsComponent { get; set; } = true;

        private List<ProductModel> products;
        private ProductModel Product { get; set; }

        private List<MealModel> meals;
        private MealModel Meal { get; set; }

        private void AddProductToMeal()
        {
            MealDetailsComponentReference.AddProductToTemporaryMeal(Product);
            //Meal.Ingredients.Add(Product);
        }

        public void MealSelectedEvent(int index)
        {
            Meal = meals[index];
            
            if(MealDetailsComponentReference is not null)
            {
                if(Meal is not null)
                {
                    Console.WriteLine($"[MealSelectedEvent] Selected meal index {index}. Meal name {Meal.Name}");
                    MealDetailsComponentReference.DisplayMealDetails(Meal);
                    HideMealDetailsComponent = false;
                }
                else
                    HideMealDetailsComponent = true;
            }
            else
            {
                Console.WriteLine($"[MealSelectedEvent] null");
                HideMealDetailsComponent = true;
            }
        }

        public void ProductSelectedEvent(int index)
        {
            Product = products[index];
        }

        public async Task OnRemoveEvent(Guid id)
        {
            Console.WriteLine($"[OnRemoveEvent] Trying to remove meal. Meal id {id}");
            var result = await _mealRepository.RemoveMealAsync(id, UserId);
            Console.WriteLine($"[OnRemoveEvent] Result: {result}");

            if(result)
            {
                _informationDialogService.ShowInformationDialog("Posiłek został usunięty.", DialogType.Success);
                Meal = null;
                var meal = meals.FirstOrDefault(m => m.Id.Equals(id));
                meals.Remove(meal);
                HideMealDetailsComponent = true;
            }
            else
                _informationDialogService.ShowInformationDialog("Wystąpił błąd nieoczekiwany błąd! Spróbuj później.", DialogType.Error);
        }

        public async Task OnSaveEvent()
        {
            Console.WriteLine($"[OnSaveEvent] Trying to save updated meal. Meal id {Meal.Id}");
            var result = await _mealRepository.UpdateMealAsync(UserId, Meal.AsMealRequestDto());
            Console.WriteLine($"[OnSaveEvent] Update Result Message: {result}");

            this.editMode = false;
            ShowSearchButton = false;
            SearchText = editMode ? "Wyszukaj produkt": "Wyszukaj posiłek";

            if(result)
                _informationDialogService.ShowInformationDialog("Posiłek został zaktualizowany.", DialogType.Success);
            else
                _informationDialogService.ShowInformationDialog("Wystąpił błąd nieoczekiwany błąd! Spróbuj później.", DialogType.Success);
        }
    
        public void OnEditModeEvent(bool editMode)
        {
            Console.WriteLine($"[OnEditModeEvent] Edit mode: {editMode}");
            SearchText = editMode ? "Wyszukaj produkt" : "Wyszukaj posiłek";
            this.editMode = editMode;
            ShowSearchButton = !editMode;
            StateHasChanged();
        }

        public async void OnSearchStringChange(string search)
        {
            Console.WriteLine($"Search string: {search}");

            if(!string.IsNullOrEmpty(search) && search.Length > 2)
            {
                if(editMode)
                    await SearchForProducts(search);
                else
                    await SearchForMeals(search);
            }
        }
        public async void OnSearchAllEvent()
        {
            var mealsDto = await _mealRepository.GetAllUserMealsAsync(UserId);

            if(mealsDto is not null)
            {
                meals = mealsDto.Select(x=>x.AsMealModel()).ToList();
                StateHasChanged();
            }
        }

        public async Task SearchForMeals(string search)
        {
            var mealsDto = await _mealRepository.GetMealByNameAsync(UserId, search);
            meals = mealsDto.Select(x=>x.AsMealModel()).ToList();
            StateHasChanged();
        }

        public async Task SearchForProducts(string search)
        {
            var productsDto = await _productRepository.GetProductsByNameAsync(UserId, search);
            products = productsDto.Select(x=>x.AsProductModel()).ToList();
            StateHasChanged();
        }
    }
}