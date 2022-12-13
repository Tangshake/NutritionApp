namespace NutritionWebClient.TokenStorageService
{
    public interface ITokenStorage
    {
        string FetchToken(string name);
        void AddToken(string key, string tokenString);

        void RemoveToken(string key);
    }
}