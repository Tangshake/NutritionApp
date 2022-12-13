using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NutritionWebClient.Dtos.Products;
using NutritionWebClient.Dtos.Products.Request;
using NutritionWebClient.Services;
using NutritionWebClient.TokenStorageService;

namespace NutritionWebClient.SyncDataService.Products
{
    public class HttpProductDataClient : HttpDataBase, IProductDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;
        private readonly JwtTokenRefreshProvider _jwtTokenRefreshProvider;
        private readonly IConfiguration _configuration;

        private readonly string _ocelotGatewayAddress;

        public HttpProductDataClient(HttpClient httpClient, ITokenStorage tokenStorage, JwtTokenRefreshProvider jwtTokenRefreshProvider, IConfiguration configuration) : base(tokenStorage, jwtTokenRefreshProvider, httpClient)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
            _jwtTokenRefreshProvider = jwtTokenRefreshProvider;
            _configuration = configuration;

            var gatewayAddress = _configuration["OcelotGateway:Address"];
            var gatewayPort = _configuration["OcelotGateway:Port"];
            _ocelotGatewayAddress = $"{gatewayAddress}:{gatewayPort}";
        }

        public async Task<ProductReadDto> GetProductByIdAsync(int userId, int id)
        {

            var finalRouteString = $"{_ocelotGatewayAddress}/api/products/by/id/{userId}/{id}";
            Console.WriteLine($"[GetProductByIdAsync] FinalRouteString: {finalRouteString}");

            var response = await SendHttpGetAsync(finalRouteString, true);
            Console.WriteLine($"[GetProductByIdAsync] Response status code: {response.StatusCode}");

            if(response is not null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var productReadDto = JsonSerializer.Deserialize<ProductReadDto>(content);
                
                Console.WriteLine($"[GetProductByIdAsync] Deserialization string: {content}");
                Console.WriteLine($"[GetProductByIdAsync] Deserialization productReadDto: {productReadDto.Name}");
                return productReadDto;
            }
            else
            {
                return null; //TODO
            }
        }

        public async Task<List<ProductReadDto>> GetProductsByNameAsync(int userId, string name)
        {
            Console.WriteLine($"[GetProductsByNameAsync] JwtToken: {_tokenStorage.FetchToken("jwt")}");
            
            var finalRouteString = $"{_ocelotGatewayAddress}/api/products/by/name/{userId}/{name}";
            Console.WriteLine($"[GetProductsByNameAsync] FinalRouteString: {finalRouteString}");

            var response = await SendHttpGetAsync(finalRouteString, true);
            Console.WriteLine($"[GetProductsByNameAsync] Response status code: {response.StatusCode}");

            if(response is not null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductReadDto>>(content);
                
                Console.WriteLine($"[GetProductByIdAsync] Deserialization string: {content}");
                
                return products;
            }
            else
            {
                return null; //TODO
            }
        }

        public async Task<bool> AddProductAsync(int userId, ProductCreateRequestDto product)
        {
            var finalRouteString = $"{_ocelotGatewayAddress}/api/products/{userId}";
            Console.WriteLine($"[AddProductAsync] FinalRouteString: {finalRouteString}");

            var httpContent = new StringContent(
                JsonSerializer.Serialize(product),
                Encoding.UTF8,
                "application/json"
            );
            
            var response = await SendHttpPostAsync(finalRouteString, httpContent, true);
            Console.WriteLine($"[AddProductAsync] Response status code: {response.StatusCode}");

            if(response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false; //TODO
            }
        }

    }
}