using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWTService.RefreshToken;
using JWTService.Repository.JwtToken;
using JWTService.Repository.RefreshToken;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JWTService.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenRepository _jwtTokenRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenManager _refreshTokenManager;

        public TokenService(IConfiguration configuration, IJwtTokenRepository jwtTokenRepository, IRefreshTokenRepository refreshTokenRepository, IRefreshTokenManager refreshTokenManager)
        {
            _configuration = configuration;
            _jwtTokenRepository = jwtTokenRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenManager = refreshTokenManager;
        }
        
        public async Task<string> CreateJwtTokenAsync(IEnumerable<Claim> claims)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //Retrieve secret key from database
            var secrets = await _jwtTokenRepository.GetJwtTokenAsync();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secrets.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var securityTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
          
            var jwtTokenExpiryTimeInMinutes = int.Parse(_configuration.GetSection("JwtToken:ExpiryTimeMinutes").Value);
            var jwtCreationDate = DateTime.UtcNow;
            var jwtExpires = DateTime.UtcNow.AddMinutes(jwtTokenExpiryTimeInMinutes);

            var token = new JwtSecurityToken(
                    issuer: "http://127.0.0.1",
                    audience: "http://127.0.0.1",
                    claims,
                    expires: jwtExpires,
                    signingCredentials: credentials
                );
            
            var tokenString = securityTokenHandler.WriteToken(token);
            Console.WriteLine($"[CreateToken] JwtToken should be created. JwtToken: {tokenString}");
            Console.WriteLine($"[CreateToken] JwtToken expires: {jwtExpires}");

            return tokenString;
        }

        public Entity.RefreshToken CreateRefreshToken(int userId)
        {
            //Create new RefreshToken
            var refreshTokenSize = int.Parse(_configuration.GetSection("RefreshToken:Size").Value);
            var refreshTokenExpiryTimeInDays = int.Parse(_configuration.GetSection("RefreshToken:ExpiryTimeDays").Value);
            var refreshTokenString = _refreshTokenManager.Generate(refreshTokenSize);
            
            var refreshToken = new Entity.RefreshToken()
            {
                UserId = userId,
                Token = refreshTokenString,
                ActivationDate = DateTime.UtcNow,
                DeactivationDate = DateTime.UtcNow.AddDays(refreshTokenExpiryTimeInDays)
            };

            Console.WriteLine($"[CreateToken] RefreshToken should be created. RefreshToken: {refreshTokenString}");
            return refreshToken;
        }
        
        public async Task<bool> ValidateExpiredJwtTokenAsync(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secrets = await _jwtTokenRepository.GetJwtTokenAsync();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secrets.SecretKey));
            
            var validatedToken = await GetClaimsPrincipalFromJwtTokenAsync(jwtToken);
            
            if(validatedToken is null)
            {
                return false;
                //throw new SecurityTokenException("--> [ValidateToken] Invalid token passed!");
            }

            return true;
        }

        public async Task<ClaimsPrincipal> GetClaimsPrincipalFromJwtTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = await GetTokenValidationParameters();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if(!ValidJwtSecurityAlgorithm(validatedToken)) 
                {
                    return null;
                }

                return principal;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        private bool ValidJwtSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }
        private async Task<TokenValidationParameters> GetTokenValidationParameters()
        {
            var secrets = await _jwtTokenRepository.GetJwtTokenAsync();
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secrets.SecretKey)),
                ValidateAudience = false,
                ValidateIssuer = false,
                RequireExpirationTime = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}