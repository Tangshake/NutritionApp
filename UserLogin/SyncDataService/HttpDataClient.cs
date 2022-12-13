using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserLogin.Dtos;

namespace UserLogin.SyncDataService
{
    public class HttpDataClient : IJwtDataClient
    {
        private readonly HttpClient _httpClient;

        public HttpDataClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<string> GetJwtTSecurityTokenAsync(UserCreditentialsDto userCreditentialsDto, string requestUri)
        {
            var httpContent = new StringContent(
              JsonSerializer.Serialize(userCreditentialsDto),
              Encoding.UTF8,
              "application/json"
            );

            var response = await _httpClient.PostAsync(requestUri, httpContent);

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[GetJwtTSecurityTokenAsync] Response status is success! We should have generated tokens now.");
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
            else
            {
                return string.Empty; //TODO
            }
        }

        public async Task<string> RefreshJwtTSecurityTokenAsync(RefreshTokenDto refreshTokenDto, string requestUri)
        {
            var httpContent = new StringContent(
              JsonSerializer.Serialize(refreshTokenDto),
              Encoding.UTF8,
              "application/json"
            );

            var response = await _httpClient.PostAsync(requestUri, httpContent);

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[RefreshJwtTSecurityTokenAsync] Response status is success! We should have refreshed token now.");
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
            else
            {
                return string.Empty; //TODO
            }
        }
    }
}