using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using NutritionWebClient.Dtos.Meal;
using NutritionWebClient.Dtos.Meal.Request;
using NutritionWebClient.Dtos.Meal.Response;
using NutritionWebClient.Services;
using NutritionWebClient.TokenStorageService;

namespace NutritionWebClient.SyncDataService.Meals
{
    public class HttpMealDataClient : HttpDataBase, IMealDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;
        private readonly JwtTokenRefreshProvider _jwtTokenRefreshProvider;
        private readonly IConfiguration _configuration;
        private readonly string _ocelotGatewayAddress;

        public HttpMealDataClient(HttpClient httpClient, ITokenStorage tokenStorage, JwtTokenRefreshProvider jwtTokenRefreshProvider, IConfiguration configuration) : base(tokenStorage, jwtTokenRefreshProvider, httpClient)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
            _jwtTokenRefreshProvider = jwtTokenRefreshProvider;
            _configuration = configuration;

            var gatewayAddress = _configuration["OcelotGateway:Address"];
            var gatewayPort = _configuration["OcelotGateway:Port"];
            _ocelotGatewayAddress = $"{gatewayAddress}:{gatewayPort}";
        }

        public async Task<string> CreateMealAsync(PredefinedMealRequestDto predefinedMealRequestDto)
        {
            var finalRouteString = $"{_ocelotGatewayAddress}/api/meals";
            Console.WriteLine($"[CreateMealAsync] FinalRouteString: {finalRouteString}");

            var httpContent = new StringContent(
                JsonSerializer.Serialize(predefinedMealRequestDto),
                Encoding.UTF8,
                "application/json"
            );

            var response = await SendHttpPostAsync(finalRouteString, httpContent, true);

            if(response is not null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }

            return string.Empty;
        }

        public async Task<PredefinedMealResponseDto> GetMealByIdAsync(int userId, string id)
        {
            var finalRouteString = $"{_ocelotGatewayAddress}/api/meals/by/id/{userId}/{id}";
            Console.WriteLine($"[GetMealByIdAsync] FinalRouteString: {finalRouteString}");

            var response = await SendHttpGetAsync(finalRouteString, true);
            Console.WriteLine($"[GetMealsByNameAsync] Response status code: {response.StatusCode}");

            if(response is not null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[GetMealByIdAsync] content: {content}");
                
                var result = JsonSerializer.Deserialize<PredefinedMealResponseDto>(content);    
                
                return result;
            }
            else
            {
                return null; //TODO
            }
        }

        public async Task<List<PredefinedMealResponseDto>> GetMealByNameAsync(int userId, string name)
        {
            var finalRouteString = $"{_ocelotGatewayAddress}/api/meals/by/name/{userId}/{name}";
            Console.WriteLine($"[GetMealByNameAsync] FinalRouteString: {finalRouteString}");

            var response = await SendHttpGetAsync(finalRouteString, true);
            Console.WriteLine($"[GetMealsByNameAsync] Response status code: {response.StatusCode}");

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[GetMealByNameAsync] content: {content}");
                
                var result = JsonSerializer.Deserialize<List<PredefinedMealResponseDto>>(content);    
                
                return result;
            }
            else
            {
                return null; //TODO
            }
        }

        public async Task<List<PredefinedMealResponseDto>> GetAllUserMealsAsync(int userId)
        {
            var finalRouteString = $"{_ocelotGatewayAddress}/api/meals/{userId}";
            Console.WriteLine($"[GetAllUserMealsAsync] FinalRouteString: {finalRouteString}");

            var response = await SendHttpGetAsync(finalRouteString, true);
            Console.WriteLine($"[GetAllUserMealsAsync] Response status code: {response.StatusCode}");

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[GetAllUserMealsAsync] content: {content}");
                
                var result = JsonSerializer.Deserialize<List<PredefinedMealResponseDto>>(content);    
                
                return result;
            }
            else
            {
                return null; //TODO
            }
        }
        public async Task<bool> UpdateMealAsync(int userId, PredefinedMealUpdateRequestDto meal)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(meal),
                Encoding.UTF8,
                "application/json"
            );
            
            var finalRouteString = $"{_ocelotGatewayAddress}/api/meals/{userId}";
            Console.WriteLine($"[GetAllUserMealsAsync] FinalRouteString: {finalRouteString}");

            var response = await SendHttpPutAsync(finalRouteString, httpContent, true);
            Console.WriteLine($"[UpdateMealAsync] Response status code: {response.StatusCode}");

            if(response is not null && response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
        
        public async Task<bool> RemoveMealAsync(Guid id, int userId)
        {
            var finalRouteString = $"{_ocelotGatewayAddress}/api/meals/{userId}/{id}";
            Console.WriteLine($"[RemoveMealAsync] FinalRouteString: {finalRouteString}");

            var response = await SendHttpDeleteAsync(finalRouteString, true);
            Console.WriteLine($"[RemoveMealAsync] Response status code: {response.StatusCode}");

            if(response is not null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[RemoveMealAsync] content: {content}");

                return true;
            }
            else
            {
                return false; //TODO
            }    
        }
    }
}