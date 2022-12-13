using System.Threading.Tasks;
using JWTService.Entity;

namespace JWTService.Repository.JwtToken
{
    public interface IJwtTokenRepository
    {
        Task<JwtTokenEntity> GetJwtTokenAsync();
    }
}