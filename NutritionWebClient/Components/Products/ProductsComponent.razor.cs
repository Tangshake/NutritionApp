using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NutritionWebClient.Dtos.Products;
using NutritionWebClient.SyncDataService.Products;

namespace NutritionWebClient.Components.Products
{

    public partial class ProductsComponent : ComponentBase
    {
        [Inject]
        public IProductDataClient _productRepository {get;set;}
        
        [Parameter]
        public int UserId { get; set; }

        private List<ProductReadDto> products;
        private ProductReadDto Product { get; set; }

        //private readonly IProductDataClient productRespository;

        public void ProductSelectedEvent(int index)
        {
            Product = products[index];
        }

        public async void OnSearchStringChange(string search)
        {
            Console.WriteLine($"Search string: {search}");

            if(!string.IsNullOrEmpty(search) && search.Length > 2)
                await SearchForProducts(search);
        }

        public async Task SearchForProducts(string search)
        {
            products = await _productRepository.GetProductsByNameAsync(UserId, search);

            StateHasChanged();
        }
    }
}