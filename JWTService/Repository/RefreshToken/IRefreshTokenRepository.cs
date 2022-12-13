using System.Threading.Tasks;
using JWTService.Entity;

namespace JWTService.Repository.RefreshToken
{
    public interface IRefreshTokenRepository
    {
        Task<int> SaveRefreshTokenAsync(JWTService.Entity.RefreshToken refreshToken);

        Task<int> RemoveRefreshTokenAsync(int tokenId, int userId);

        Task<Entity.RefreshToken> GetRefreshTokenAsync(string userName);
    }
}