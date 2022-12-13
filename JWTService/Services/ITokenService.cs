using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JWTService.Services
{
    public interface ITokenService
    {
        Task<string> CreateJwtTokenAsync(IEnumerable<Claim> claims);

        Entity.RefreshToken CreateRefreshToken(int userId);

        Task<bool> ValidateExpiredJwtTokenAsync(string jwtToken);

        public Task<ClaimsPrincipal> GetClaimsPrincipalFromJwtTokenAsync(string token);
    }
}