using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NutritionWebClient.Model.Meal;
using NutritionWebClient.Model.Product;

namespace NutritionWebClient.Components.Meal.Browse.SingleMeal
{
    public partial class MealDetails : ComponentBase
    {
        [Parameter]
        public EventCallback<bool> OnEditMode { get; set; }

        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public EventCallback<Guid> OnRemove { get; set; }

        [Parameter]
        public MealModel Meal { get; set; }

        private MealModel TemporaryMeal { get; set; }

        [Parameter]
        public bool ShowEditButton { get; set; }
        private bool ShowCustomModalDialog { get; set; } = false;
        private string CustomDialogDetails { get; set; }

        private string EditButtonRoleText = "Edytuj";
        
        private bool editEnable = false;

        private string SaveDisabled { get; set; } = "disabled";

        private MealSummary Summary = new MealSummary();

        public void DisplayMealDetails(MealModel meal)
        {
            Console.WriteLine($"[DisplayMealDetails] Meal name: {meal.Name}. Meal contains {meal.Ingredients.Count} ingredients.");
            Meal = meal;
            CreateTemporaryMeal();
            CalculateMealSummary();
            StateHasChanged();
        }

        // This method is called by Parent component: MealBrowserComponent
        public void AddProductToTemporaryMeal(ProductModel product)
        {
            Console.WriteLine($"[AddProductToTemporaryMeal] Product name: {product.Name}. Meal contains {TemporaryMeal.Ingredients.Count} ingredients.");
            TemporaryMeal.Ingredients.Add(product);
            Console.WriteLine($"[AddProductToTemporaryMeal] Product added. Meal contains {TemporaryMeal.Ingredients.Count} ingredients.");
            CalculateMealSummary();
        }

        private void OnSaveUpdatedPredefinedMeal()
        {
            Console.WriteLine($"Temporary meal name: {TemporaryMeal.Name}");
            UpdateMealModelOnSave();
            TurnOffEditMode();
            OnSave.InvokeAsync();
        }

        private async Task OnEditButtonClick()
        {
            editEnable = !editEnable;
            
            (editEnable ? (Action)TurnOnEditMode : TurnOffEditMode)();

            await OnEditMode.InvokeAsync(editEnable);
        }

        private void TurnOnEditMode()
        {
            editEnable = true;
            EditButtonRoleText = "Anuluj";
            SaveDisabled = "";
        }

        private void TurnOffEditMode()
        {
            editEnable = false;
            EditButtonRoleText = "Edytuj";
            SaveDisabled = "disabled";
        }

        private void OnRemovePredefinedMeal()
        {
            ShowCustomModalDialog = true;
            CustomDialogDetails = $"Nazwa posiłku: {TemporaryMeal.Name}";
        }

        private void OnProductRemoveEvent(int index)
        {
            Console.WriteLine($"--> [OnProductRemoveEvent] Removing product at index {index}. There are {TemporaryMeal.Ingredients.Count} ingredients on the list.");
            TemporaryMeal.Ingredients.RemoveAt(index);
            Console.WriteLine($"--> [OnProductRemoveEvent] Removed product at index {index}. After removing there list contains {TemporaryMeal.Ingredients.Count} ingredients.");
            CalculateMealSummary();
        }

        private void OnNameChange(ChangeEventArgs args)
        {
            Console.WriteLine($"[OnNameChange] {args.Value.ToString()}");
        }

        private void OnWeightChange(ChangeEventArgs args, int index)
        {
            Console.WriteLine($"[OnWeightChange] {args.Value.ToString()}");
            var weight = float.Parse(args.Value.ToString());

            if(weight <= 1)
            {
                TemporaryMeal.Ingredients[index].Weight = 1;
                StateHasChanged();
            }
            else if(weight >= 10000)
            {
                TemporaryMeal.Ingredients[index].Weight = 10000;
                StateHasChanged();
            }
            else
            {
                TemporaryMeal.Ingredients[index].Weight = float.Parse(args.Value.ToString());
                CalculateMealSummary();
                StateHasChanged();
            }
        }

        private void CalculateMealSummary()
        {
            if(Summary is null) Summary = new MealSummary();
            
            Summary.ClearAll();
            if(TemporaryMeal is not null && TemporaryMeal.Ingredients is not null && TemporaryMeal.Ingredients.Count > 0)
            {
                foreach(var product in TemporaryMeal.Ingredients)
                {
                    Summary.Kcal += (product.Kcal * product.Weight) / 100;
                    Summary.Weight += product.Weight;
                    Summary.Protein += (product.Protein * product.Weight) / 100;
                    Summary.Carbohydrates += (product.Carbohydrates * product.Weight) / 100;
                    Summary.Fat += (product.Fat * product.Weight) / 100;
                    Summary.Roughage += (product.Roughage * product.Weight) / 100;
                }

            }
        }

        private void CreateTemporaryMeal()
        {
            TemporaryMeal = new MealModel()
            {
                Id = Meal.Id,
                Name = Meal.Name,
                UserId = Meal.UserId,
                Ingredients = new List<ProductModel>(Meal.Ingredients)
            };
        }

        private void UpdateMealModelOnSave()
        {
            Console.WriteLine($"[UpdateMealModelOnSave] Meal contains {TemporaryMeal.Ingredients.Count} ingredients.");
            Meal.Name = TemporaryMeal.Name;
            Meal.Ingredients = new List<ProductModel>(TemporaryMeal.Ingredients);
        }

        private async Task OnDialogClose(bool result)
        {
            ShowCustomModalDialog = false;
            if(result)
                await OnRemove.InvokeAsync(Meal.Id);
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
}