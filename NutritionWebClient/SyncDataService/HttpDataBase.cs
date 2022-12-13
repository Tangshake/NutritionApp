using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NutritionWebClient.Services;
using NutritionWebClient.TokenStorageService;

namespace NutritionWebClient.SyncDataService
{
    public abstract class HttpDataBase
    {
        private readonly ITokenStorage _tokenStorage;
        private readonly JwtTokenRefreshProvider _jwtTokenRefreshProvider;
        private readonly HttpClient _httpClient;

        protected HttpDataBase(ITokenStorage tokenStorage, JwtTokenRefreshProvider jwtTokenRefreshProvider, HttpClient httpClient)
        {
            _tokenStorage = tokenStorage;
            _jwtTokenRefreshProvider = jwtTokenRefreshProvider;
            _httpClient = httpClient;
        }

        public virtual async Task RefreshTokenIfNeeded()
        {
            Console.WriteLine("[RefreshTokenIfNeeded] Checking if jwt token needs to be refreshed...");
            var jwtRefreshNeeded = JwtTokenHelper.JwtHelper.Expired(_tokenStorage.FetchToken("jwt"));
            Console.WriteLine($"[RefreshTokenIfNeeded] Expired: {jwtRefreshNeeded}");

            if(jwtRefreshNeeded)
                await _jwtTokenRefreshProvider.Refresh();
        }

        public virtual async Task<HttpResponseMessage> SendHttpPostAsync(string url, StringContent content, bool refresh)
        {
            if(refresh)
            {
                try
                {
                    await RefreshTokenIfNeeded();
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStorage.FetchToken("jwt"));
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"---EX---> [SendHttpPostAsync] {ex.Message}");
                    return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
            }

            try
            {
                Console.WriteLine($"[SendHttpPostAsync] Sending http message...");
                return await _httpClient.PostAsync(url, content);    
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"---EX---> [SendHttpPostAsync] {ex}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }

        public virtual async Task<HttpResponseMessage> SendHttpGetAsync(string url, bool refresh)
        {
            if(refresh)
            {
                try
                {
                    await RefreshTokenIfNeeded();
                    var token = _tokenStorage.FetchToken("jwt");
                    
                    if(string.IsNullOrEmpty(token))
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"---EX---> [SendHttpGetAsync:RefreshJwtToken] {ex.Message}");
                    return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
            }
            try
            {
                Console.WriteLine($"[SendHttpGetAsync] Sending http message...");
                return await _httpClient.GetAsync(url);    
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"---EX---> [SendHttpGetAsync] {ex.Message}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }

        public virtual async Task<HttpResponseMessage> SendHttpPutAsync(string url, StringContent content, bool refresh)
        {
            if(refresh)
            {
                try
                {
                    await RefreshTokenIfNeeded();
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStorage.FetchToken("jwt"));
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"---EX---> [SendHttpPutAsync] {ex.Message}");
                    return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
            }
            try
            {
                Console.WriteLine($"[SendHttpPutAsync] Sending http message...");
                return await _httpClient.PutAsync(url, content);    
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"---EX---> [SendHttpPutAsync] {ex.Message}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }

        public virtual async Task<HttpResponseMessage> SendHttpDeleteAsync(string url, bool refresh)
        {
            if(refresh)
            {
                try
                {
                    await RefreshTokenIfNeeded();
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStorage.FetchToken("jwt"));
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"---EX---> [SendHttpDeleteAsync] {ex.Message}");
                    return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
            }
            try
            {
                Console.WriteLine($"[SendHttpDeleteAsync] Sending http message...");
                return await _httpClient.DeleteAsync(url);    
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"---EX---> [SendHttpDeleteAsync] {ex.Message}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}