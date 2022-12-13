using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace NutritionWebClient.JwtTokenHelper
{
    public static class JwtHelper
    {
        public static string ExctractClaim(string token, string claimType)
        {
            if(!string.IsNullOrEmpty(token) && claimType != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var jsonToken = tokenHandler.ReadJwtToken(token);
                var tokenS = jsonToken as JwtSecurityToken;

                var claim = tokenS.Claims.First(claim => claim.Type == claimType);

                if(claim is null)
                    return string.Empty;

                return claim.Value;
            }

            return string.Empty;
        }
        
        public static bool Expired(string token)
        {
            if(string.IsNullOrEmpty(token))
                return true;

            var exp = ExctractClaim(token, "exp");

            var expireDate = DateTimeOffset.FromUnixTimeSeconds(int.Parse(exp)).LocalDateTime;

            if(DateTime.Now.ToLocalTime() > expireDate)
                return true;

            return false;
        }
    }
}