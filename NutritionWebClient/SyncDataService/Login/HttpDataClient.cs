using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NutritionWebClient.Dtos;
using NutritionWebClient.Dtos.RefreshToken;
using NutritionWebClient.Dtos.User;
using NutritionWebClient.Services;
using NutritionWebClient.TokenStorageService;

namespace NutritionWebClient.SyncDataService.Login
{
    public class HttpDataClient : HttpDataBase, ILoginDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ITokenStorage _tokenStorage;
        private readonly string _ocelotGatewayAddress;

        public HttpDataClient(HttpClient httpClient, IConfiguration configuration, ITokenStorage tokenStorage, JwtTokenRefreshProvider jwtTokenRefreshProvider) : base(tokenStorage, jwtTokenRefreshProvider, httpClient)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _tokenStorage = tokenStorage;

            var gatewayAddress = _configuration["OcelotGateway:Address"];
            var gatewayPort = _configuration["OcelotGateway:Port"];
            _ocelotGatewayAddress = $"{gatewayAddress}:{gatewayPort}";
        }

        public async Task<EmailVerificationResponseDto> SendEmailVerificationData(EmailVerificationDto emailVerificationDto)
        {
            var finalRouteString = $"{_ocelotGatewayAddress}/api/verifyemail";
            Console.WriteLine($"[SendEmailVerificationData] FinalRouteString: {finalRouteString}");

            var httpContent = new StringContent(
                JsonSerializer.Serialize(emailVerificationDto),
                Encoding.UTF8,
                "application/json"
            );

            // var response = await _httpClient.PostAsync(finalRouteString, httpContent);
            var response =  await SendHttpPostAsync(finalRouteString, httpContent, false);

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var emailVerificationResponseDto = JsonSerializer.Deserialize<EmailVerificationResponseDto>(content);
                
                return emailVerificationResponseDto;
            }
            else
            {
                return null; //TODO
            }
        }

        public async Task<RegisterResponseDto> SendUserRegistrationData(UserRegisterDto userRegisterDto)
        {
            var finalRouteString = $"{_ocelotGatewayAddress}/api/register";
            Console.WriteLine($"[SendUserRegistrationData] FinalRouteString: {finalRouteString}");

            var httpContent = new StringContent(
                JsonSerializer.Serialize(userRegisterDto),
                Encoding.UTF8,
                "application/json"
            );

            // var response = await _httpClient.PostAsync(finalRouteString, httpContent);
            var response =  await SendHttpPostAsync(finalRouteString, httpContent, false);

            if(response is not null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var registerResponseDto = JsonSerializer.Deserialize<RegisterResponseDto>(content);
                
                return registerResponseDto;
            }
            else
            {
                return null; //TODO
            }
        }

        public async Task<string> SendLoginUserData(UserLoginDto user)
        {
            var finalRouteString = $"{_ocelotGatewayAddress}/api/login";
            Console.WriteLine($"[GetProductByIdAsync] FinalRouteString: {finalRouteString}");

            var httpContent = new StringContent(
                JsonSerializer.Serialize(user),
                Encoding.UTF8,
                "application/json"
            );

            // var response = await _httpClient.PostAsync(finalRouteString, httpContent);
            var response =  await SendHttpPostAsync(finalRouteString, httpContent, false);

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> RefreshJwtTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var finalRouteString = $"{_ocelotGatewayAddress}/api/refresh";
            Console.WriteLine($"[GetProductByIdAsync] FinalRouteString: {finalRouteString}");

            var httpContent = new StringContent(
                JsonSerializer.Serialize(refreshTokenDto),
                Encoding.UTF8,
                "application/json"
            );

            // var response = await _httpClient.PostAsync(finalRouteString, httpContent);
            var response =  await SendHttpPostAsync(finalRouteString, httpContent, false);

            if(response is not null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }

            return string.Empty;
        }

        public async Task<UserSecurityStampResponse> GetUserSecurityStamp(UserSecurityStampRequest userSecurityStampRequest)
        {
            // await RefreshTokenIfNeeded();
            // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStorage.FetchToken("jwt"));

            var finalRouteString = $"{_ocelotGatewayAddress}/api/secstamp";
            Console.WriteLine($"[GetUserSecurityStamp] FinalRouteString: {finalRouteString}");

            var httpContent = new StringContent(
                JsonSerializer.Serialize(userSecurityStampRequest),
                Encoding.UTF8,
                "application/json"
            );

            // var response = await _httpClient.PostAsync(finalRouteString, httpContent);
            var response =  await SendHttpPostAsync(finalRouteString, httpContent, true);

            if(response is not null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userSecurityStamp = JsonSerializer.Deserialize<UserSecurityStampResponse>(content);

                return userSecurityStamp;
            }

            return null;
        }

        public async Task UpdateUserSecurityStamp(UpdateUserSecurityStampRequest updateSecurityStampRequest)
        {
            // await RefreshTokenIfNeeded();
            // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStorage.FetchToken("jwt"));
            
            var finalRouteString = $"{_ocelotGatewayAddress}/api/secstampupdate";
            Console.WriteLine($"[UpdateUserSecurityStamp] FinalRouteString: {finalRouteString}");

            var httpContent = new StringContent(
                JsonSerializer.Serialize(updateSecurityStampRequest),
                Encoding.UTF8,
                "application/json"
            );

            await SendHttpPostAsync(finalRouteString, httpContent, true);
        }

        
    }
}