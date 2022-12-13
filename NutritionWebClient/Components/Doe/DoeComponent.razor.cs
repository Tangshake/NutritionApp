using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using NutritionWebClient.Components.Doe.SingleHour;
using NutritionWebClient.Dtos.Doe.Response;
using NutritionWebClient.Dtos.Products;
using NutritionWebClient.Model.Doe.DoeNutritionSummary;
using NutritionWebClient.Model.Meal;
using NutritionWebClient.Model.Product;
using NutritionWebClient.Services.InformationDialog;
using NutritionWebClient.SyncDataService.Does;
using NutritionWebClient.SyncDataService.Meals;
using NutritionWebClient.SyncDataService.Products;

namespace NutritionWebClient.Components.Doe
{
    public partial class DoeComponent : ComponentBase
    {
        [Inject]
        public IMapper _mapper { get; set; }
        
        [Inject]
        public IProductDataClient _productRepository {get;set;}

        [Inject]
        public IMealDataClient _mealRepository { get; set; }
        
        [Inject]
        public IDoeDataClient _doeRepository { get; set; }

        [Inject]
        public InformationDialogService _informationDialogService { get; set; }
        
        [Parameter]
        public int UserId { get; set; }

        private List<SingleHourComponent> singleHourComponents = new List<SingleHourComponent>();

        private List<MealModel> meals;
        private MealModel Meal { get; set; }

        private List<ProductModel> products;
        public ProductModel Product { get; set; } = new ProductModel();

        private DoeResponseDto doeResponseDto;
        private DateTime CurrentDisplayedDoeDate = DateTime.Now.ToUniversalTime();//Parse("2012-03-20T00:00:00.000+00:00").ToUniversalTime();

        private Summary DaySummary;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                await SearchForDoe(CurrentDisplayedDoeDate);
                OnNutritionSummaryChangeEvent();
            }
        }

        public async Task SearchForDoe(DateTime date)
        {
            Console.WriteLine($"[SerachForDoe] Searching for doe with date: {date}");
            singleHourComponents.Clear();
            doeResponseDto = await _doeRepository.GetDoeByDateAsync(UserId, date);

            if(doeResponseDto is null)
                DaySummary = null;
        }
        
        private void OnNutritionSummaryChangeEvent()
        {
            var summary = new Summary();
            Console.WriteLine($"[OnNutritionSummaryChangeEvent] Number of cards: {singleHourComponents.Count}");

            foreach(var component in singleHourComponents)
            {
                Console.WriteLine($"[OnNutritionSummaryChangeEvent] Hour: {component.OryginalSingleEntry.Hour}");
                if(component is not null && summary is not null)
                {
                    summary.Kcal += component.HourSummary.Kcal;
                    summary.Protein += component.HourSummary.Protein;
                    summary.Carbohydrates += component.HourSummary.Carbohydrates;
                    summary.Fat += component.HourSummary.Fat;
                    summary.Roughage += component.HourSummary.Roughage;
                    summary.Weight += component.HourSummary.Weight;
                }
            }

            DaySummary = summary;
            StateHasChanged();

            Console.WriteLine($"[OnNutritionSummaryChangeEvent] {summary.Kcal} {summary.Protein} {summary.Carbohydrates} {summary.Fat} {summary.Roughage} {summary.Weight} ");
        }

        //SAVE DATA: Event callback from ChildComponent -> SingleHourComponent
        private async void OnEditSaveButtonClickEvent()
        {
            Console.WriteLine($"[OnEditSaveButtonClickEvent] User requested to save the doe.");
            var result = await _doeRepository.UpdateDoeAsync(UserId, CurrentDisplayedDoeDate.Date, doeResponseDto.AsDoeRequestDto());
            
            Console.WriteLine($"[OnEditSaveButtonClickEvent] Update Doe result: {result}");
            if(result)
            {
                _informationDialogService.ShowInformationDialog("Aktualizacja zakończona sukcesem.", DialogType.Success);
            }
            else
            {
                _informationDialogService.ShowInformationDialog("Aktualizacja zakończona niepowodzeniem.", DialogType.Error);
            }
        }

        //REMOVE HOUR CARD: Event callback from ChildComponent -> SingleHourComponent
        private async Task OnRemoveCardEvent(SingleHourComponent component)
        {
            Console.WriteLine($"[OnRemoveCardEvent] Request to remove Card.");
 
            Console.WriteLine($"[OnRemoveCardEvent] There is/are: {doeResponseDto.Does.Count} doe/s.");
            Console.WriteLine("Removing...");
            doeResponseDto.Does.Remove(component.OryginalSingleEntry);
            var removed = singleHourComponents.Remove(component);
            if(removed)
                Console.WriteLine($"[OnRemoveCardEvent] Reference was removed. List of references contains: {singleHourComponents.Count} elements.");

            Console.WriteLine($"[OnRemoveCardEvent] There is/are: {doeResponseDto.Does.Count} doe/s.");

            var result = await _doeRepository.UpdateDoeAsync(UserId, CurrentDisplayedDoeDate, doeResponseDto.AsDoeRequestDto());
            Console.WriteLine($"[OnRemoveCardEvent] Update Doe result: {result}");
        }

        //Event callback from Child Component: SingleHourComponent
        private void OnCardClickEvent(SingleHourComponent card)
        {
            Console.WriteLine($"[OnCardClickEvent] Card was clicked!");
            if(singleHourComponents is not null && singleHourComponents.Count > 0)
            {
                foreach(var component in singleHourComponents)
                {
                    if(component == card)
                        component.SelectCard();
                    else
                        component.DeselectCard();
                }
            }
        }

        private void OnAddNewCardButtonClick()
        {
            Console.WriteLine("[OnAddNewCardButtonClick] Adding empty card.");
            if(doeResponseDto is not null)
            {
                doeResponseDto.Does.Add(
                    new SingleEntryDto() 
                    {
                        Hour = "00:00",
                        Meals = new List<SingleMealDto>(),
                        Products = new List<SingleProductDto>()
                    });
            }
            else if (doeResponseDto is null)
            {
                Console.WriteLine("[OnAddNewCardButtonClick] doeResponseDto is null.");
                doeResponseDto = new DoeResponseDto();
                doeResponseDto.Does = new List<SingleEntryDto>() {
                    new SingleEntryDto() 
                    {
                        Hour = "00:00",
                        Meals = new List<SingleMealDto>(),
                        Products = new List<SingleProductDto>()
                    }};

                doeResponseDto.Date = CurrentDisplayedDoeDate;
                doeResponseDto.UserId = UserId;
                StateHasChanged();
            }
        }

        private async Task OnNextDayButtonClick()
        {
            if(CurrentDisplayedDoeDate.AddDays(1) <= DateTime.Now)
            {
                CurrentDisplayedDoeDate = CurrentDisplayedDoeDate.AddDays(1);
                await SearchForDoe(CurrentDisplayedDoeDate);
            }
        }

        private async Task OnPreviousDayButtonClick()
        {
            CurrentDisplayedDoeDate = CurrentDisplayedDoeDate.AddDays(-1);
            await SearchForDoe(CurrentDisplayedDoeDate);
        }
        
        private async Task OnDoeDateChange(ChangeEventArgs args)
        {
            Console.WriteLine($"[OnDoeDateChange] {args.Value}");
            var dateString = DateTime.Parse(args.Value.ToString()).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK'Z'");
            Console.WriteLine($"[OnDoeDateChange] {dateString}");

            CurrentDisplayedDoeDate = DateTime.Parse(dateString).ToUniversalTime();

            await SearchForDoe(CurrentDisplayedDoeDate);
        }

        public async Task OnProductSearchStringChange(string search)
        {
            Console.WriteLine($"Search string: {search}");

            if(!string.IsNullOrEmpty(search) && search.Length > 2)
                 await SearchForProducts(search);
        }

        public async Task OnMealSearchStringChange(string search)
        {
            Console.WriteLine($"Search string: {search}");
            // Information = ShowInfo.NoProduct;

            if(!string.IsNullOrEmpty(search) && search.Length > 2)
                await SearchForMeals(search);
        }

        public void ProductSelectedEvent(int index)
        {
            var productModel = products[index];
            var product = _mapper.Map<SingleProductDto>(productModel);

            if(product is not null && product.Product is not null) 
            {
                //Search for the active (selected) card that is in edit mode
                var activeCard = singleHourComponents.FirstOrDefault(c => c.Selected && c.EditModelEnabled);

                if(activeCard is not null)
                activeCard.AddProduct(product);
            }

        }

        public void MealSelectedEvent(int index)
        {
            var mealModel = meals[index];
            var meal = _mapper.Map<SingleMealDto>(mealModel);

            if(meal is not null && meal.Meal is not null) 
            {
                Console.WriteLine("Nothing is null");

                //Search for the active (selected) card that is in edit mode
                var activeCard = singleHourComponents.FirstOrDefault(c => c.Selected && c.EditModelEnabled);
                
                //Set proper weight
                var weight = GetMealWeight(meal);
                meal.Weight = (int)weight;

                if(activeCard is not null)
                    activeCard.AddMeal(meal);
            }
        }

        public async void OnSearchAllEvent()
        {
            var mealsDto = await _mealRepository.GetAllUserMealsAsync(UserId);
            meals = mealsDto.Select(x=>x.AsMealModel()).ToList();

            StateHasChanged();
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

        private float GetMealWeight(SingleMealDto meal)
        {
            if(meal is not null && meal.Meal is not null && meal.Meal.Ingredients is not null)
            {
                float weight = 0;
                foreach(var item in meal.Meal.Ingredients)
                {
                    weight += item.Weight;
                }

                return weight;
            }

            return 0;
        }

        public SingleHourComponent Ref {
            set {
                    singleHourComponents.Add(value);
                    Console.WriteLine($"[Ref] Added component reference with index {value.CardIndex}");
                    OnNutritionSummaryChangeEvent();            
                }
        }
    }
}