using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NutritionWebClient.Components.Meal.Create;
using NutritionWebClient.Model.Product;
using NutritionWebClient.Services.InformationDialog;
using NutritionWebClient.SyncDataService.Products;

namespace NutritionWebClient.Components.Products.Create
{
    public partial class ProductCreatorComponent : ComponentBase
    {
        [Inject]
        public IProductDataClient _productRepository { get; set; }
        
        [Inject]
        public InformationDialogService _informationDialogService { get; set; }
        
        [Parameter]
        public int UserId { get; set; }

        public ProductModel Product { get; set; } = new ProductModel();
        
        private ShowInfo Information = ShowInfo.NoProduct;
        private bool ShowSavingSpinner { get; set; } = false;


        public async Task SaveProduct()
        {
            if(Product is not null && !string.IsNullOrEmpty(Product.Name))
            {
                ShowSavingSpinner = true;
                var productCreateRequestDto = Product.AsProductCreateRequestDto();
                PrintProductToConsole(productCreateRequestDto);
                var result = await _productRepository.AddProductAsync(UserId, productCreateRequestDto);

                if(result)
                {
                    _informationDialogService.ShowInformationDialog("Twój produkt został dodany!.", DialogType.Success);
                    Information = ShowInfo.MealSuccess;

                    Product = new ProductModel();
                }
                else
                {
                    Information = ShowInfo.MealFail;
                    _informationDialogService.ShowInformationDialog("Ups! Coś poszło nie tak.", DialogType.Error);
                }
            }
            else
            {
                Information = ShowInfo.MealFail;
                _informationDialogService.ShowInformationDialog("Nazwa produktu nie może być pusta!", DialogType.Error);
            }

            ShowSavingSpinner = false;
        }

        private void PrintProductToConsole(object obj)
        {
            System.Reflection.MemberInfo[] memberInfo;

            Type typeOfItem = obj.GetType();
            memberInfo = typeOfItem.GetMembers();

            foreach(var member in memberInfo)
            {
                if(member.MemberType == System.Reflection.MemberTypes.Property)
                {
                    Console.WriteLine($"{member.Name}:{obj.GetType().GetProperty(member.Name).GetValue(obj)}");
                }
            }
        }
    }
}