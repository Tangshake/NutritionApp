using Microsoft.Extensions.Caching.Memory;

namespace NutritionWebClient.TokenStorageService
{
    public class TokenStorage : ITokenStorage
    {
        private readonly IMemoryCache _memoryCache;

        public TokenStorage(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public string FetchToken(string name)
        {
            return _memoryCache.Get<string>(name);
        }

        public void AddToken(string key, string tokenString)
        {
            _memoryCache.Set<string>(key, tokenString);
        }

        public void RemoveToken(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}