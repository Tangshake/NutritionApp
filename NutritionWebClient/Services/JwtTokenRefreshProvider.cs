using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NutritionWebClient.Dtos.RefreshToken;
using NutritionWebClient.TokenStorageService;

namespace NutritionWebClient.Services
{
    public class JwtTokenRefreshProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;
        private readonly IConfiguration _configuration;
        private readonly string _ocelotGatewayAddress;

        public JwtTokenRefreshProvider(HttpClient http, ITokenStorage tokenStorage, IConfiguration configuration)
        {
            _httpClient = http;
            _tokenStorage = tokenStorage;
            _configuration = configuration;

            var gatewayAddress = _configuration["OcelotGateway:Address"];
            var gatewayPort = _configuration["OcelotGateway:Port"];
            _ocelotGatewayAddress = $"{gatewayAddress}:{gatewayPort}";
        }

        public async Task Refresh()
        {
            Console.WriteLine($"[JwtTokenRefreshProvider] Refreshing JwtToken: {DateTime.Now}");

            var jwtString = _tokenStorage.FetchToken("jwt");
            if(string.IsNullOrEmpty(jwtString))
                return;

            var refreshString = _tokenStorage.FetchToken("refresh");
            if(string.IsNullOrEmpty(refreshString))
                return;

            var userEmail = JwtTokenHelper.JwtHelper.ExctractClaim(jwtString, ClaimTypes.Email);
            var userId = int.Parse(JwtTokenHelper.JwtHelper.ExctractClaim(jwtString, "id"));
            var refreshJwtTokenResult = await RefreshJwtTokenAsync(new Dtos.RefreshToken.RefreshTokenDto(){ JwtToken = jwtString, RefreshToken = refreshString, UserEmail = userEmail, UserId = userId });

            if(!string.IsNullOrEmpty(refreshJwtTokenResult))
            {
                _tokenStorage.RemoveToken("jwt");
                _tokenStorage.RemoveToken("refresh");

                var tokensReadDto = JsonSerializer.Deserialize<TokensReadDto>(refreshJwtTokenResult);
                Console.WriteLine($"--> [Refresh] JwtTokenString: {tokensReadDto.JwtToken}");
                Console.WriteLine($"--> [Refresh] RefreshTokenString: {tokensReadDto.RefreshToken}");

                _tokenStorage.AddToken("jwt", tokensReadDto.JwtToken);
                _tokenStorage.AddToken("refresh", tokensReadDto.RefreshToken);

                Console.WriteLine($"Tokens added to cache");
            }
            else
            {
                Console.WriteLine($"-----EX-----> [Refresh] Jwt: {jwtString} | Refresh: {refreshString} | refreshJwtTokenResult {refreshJwtTokenResult}");
            }
        }

        private async Task<string> RefreshJwtTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var finalRouteString = $"{_ocelotGatewayAddress}/api/refresh";
            Console.WriteLine($"[GetProductByIdAsync] FinalRouteString: {finalRouteString}");

            var httpContent = new StringContent(
                JsonSerializer.Serialize(refreshTokenDto),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(finalRouteString, httpContent);

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }

            return string.Empty;
        }
    }
}