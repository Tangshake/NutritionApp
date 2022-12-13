using System.Collections.Generic;
using System.Security.Claims;
using JWTService.Dtos.Response;
using JWTService.Dtos.Request;

namespace JWTService
{
    public static class Extensions
    {
        // public static RefreshTokenIDto AsDto(this JWTService.Entity.RefreshToken refreshTokenEntity)
        // {
        //     return new RefreshTokenIDto()
        //     {
        //         UserId = refreshTokenEntity.UserId,
        //         UserEmail = refreshTokenEntity.
                
        //     };
        // }

        public static IEnumerable<Claim> AsClaims(this JwtTokenCreateRequestDto userIdentityIDto)
        {
            return new Claim[]
            {
                new Claim(ClaimTypes.Name, userIdentityIDto.Name),
                new Claim(ClaimTypes.Email, userIdentityIDto.Email),
                new Claim(ClaimTypes.Role, userIdentityIDto.Role),
                new Claim("id", userIdentityIDto.Id.ToString())
            };
        }
    }
}