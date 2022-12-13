namespace JWTService.RefreshToken
{
    public interface IRefreshTokenManager
    {
        string Generate(int length);
    }
}