using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using NutritionWebClient.Dtos.Doe.Request;
using NutritionWebClient.Dtos.Doe.Response;
using NutritionWebClient.Dtos.Meal;
using NutritionWebClient.Dtos.Meal.Request;
using NutritionWebClient.Dtos.Meal.Response;
using NutritionWebClient.Services;
using NutritionWebClient.TokenStorageService;

namespace NutritionWebClient.SyncDataService.Does
{
    public class HttpDoeDataClient : HttpDataBase, IDoeDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;
        private readonly JwtTokenRefreshProvider _jwtTokenRefreshProvider;
        private readonly IConfiguration _configuration;
        private readonly string _ocelotGatewayAddress;
        
        public HttpDoeDataClient(HttpClient httpClient, ITokenStorage tokenStorage, JwtTokenRefreshProvider jwtTokenRefreshProvider, IConfiguration configuration) : base(tokenStorage, jwtTokenRefreshProvider, httpClient)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
            _jwtTokenRefreshProvider = jwtTokenRefreshProvider;
            _configuration = configuration;

            var gatewayAddress = _configuration["OcelotGateway:Address"];
            var gatewayPort = _configuration["OcelotGateway:Port"];
            _ocelotGatewayAddress = $"{gatewayAddress}:{gatewayPort}";
        }

        public async Task<DoeResponseDto> GetDoeByDateAsync(int userId, DateTime date)
        {
            var universalDateString = date.Date.ToString("o");

            var finalRouteString = $"{_ocelotGatewayAddress}/api/myday/{userId}/{universalDateString}";
            Console.WriteLine($"[GetDoeByDateAsync] FinalRouteString: {finalRouteString}");

            var response = await SendHttpGetAsync(finalRouteString, true);
            Console.WriteLine($"[GetDoeByDateAsync] Response status code: {response.StatusCode}");    

            if(response is not null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[GetDoeByDateAsync] content: {content}");
                
                var result = JsonSerializer.Deserialize<DoeResponseDto>(content);    
                Console.WriteLine($"[GetDoeByDateAsync] result: {result}");

                return result;
            }
            else
            {
                return null; //TODO
            }
        }

        public async Task<bool> UpdateDoeAsync(int userId, DateTime date, DoeRequestDto doeRequestDto)
        {
            var universalDateString = date.Date.ToString("o");
            
            var finalRouteString = $"{_ocelotGatewayAddress}/api/myday/{userId}/{universalDateString}";
            Console.WriteLine($"[UpdateDoeAsync] DoeDate: {doeRequestDto.Date}");
            Console.WriteLine($"[UpdateDoeAsync] FinalRouteString: {finalRouteString}");
            

            var httpContent = new StringContent(
                JsonSerializer.Serialize(doeRequestDto),
                Encoding.UTF8,
                "application/json"
            );

            var response = await SendHttpPutAsync(finalRouteString, httpContent, true);
            Console.WriteLine($"[UpdateDoeAsync] Response status code: {response.StatusCode}");

            if(response is not null && response.IsSuccessStatusCode)
            {
                if(response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[UpdateDoeAsync] content: {content}");
                    
                    var result = JsonSerializer.Deserialize<DoeResponseDto>(content);    
                    
                    return true;
                }

                return true;
            }
            else
            {
                return false; //TODO
            }
        }
    }
}