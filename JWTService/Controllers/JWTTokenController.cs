using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using JWTService.Dtos.Request;
using JWTService.Dtos.Response;
using JWTService.Dtos;
using JWTService.RefreshToken;
using JWTService.Repository.RefreshToken;
using JWTService.Repository.JwtToken;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JWTService.Services;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using JWTService.AsyncDataServices.RabbitMQ;
using JWTService.Dtos.RabbitMQ.Request;
using Microsoft.AspNetCore.Http;

namespace JWTService.Controllers
{
    [ApiController]
    [Route("/generatetoken")]
    public class JWTTokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenRepository _tokenRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenManager _refreshTokenManager;
        private readonly IMessageBusClient _messageBusClient;

        public JWTTokenController(IConfiguration configuration, IJwtTokenRepository tokenRepository, IRefreshTokenRepository refreshTokenRepository, ITokenService tokenService, IRefreshTokenManager refreshTokenManager, IMessageBusClient messageBusClient)
        {
            _configuration = configuration;
            _tokenRepository = tokenRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _refreshTokenManager = refreshTokenManager;
            _messageBusClient = messageBusClient;
        }
        
        [HttpPost]
        [Produces("application/json")]
        [Route("/api/create")]
        public async Task<IActionResult> CreateTokenAsync([FromBody] JwtTokenCreateRequestDto jwtTokenCreateRequestDto)
        {
            Console.WriteLine($"[CreateToken] We got request for generating JWTToken and RefreshToken for user {jwtTokenCreateRequestDto.Name} {jwtTokenCreateRequestDto.Email} {jwtTokenCreateRequestDto.Id}");
            var logDto = new LogRequestDto(){ Date = DateTime.Now, UserId = jwtTokenCreateRequestDto.Id, ServiceName = "JwtService", Method = "CreateToken", Message = $"Creating refresh and jwt tokens."};

            //Create JwtToken first
            var jwtToken = await _tokenService.CreateJwtTokenAsync(jwtTokenCreateRequestDto.AsClaims());
            
            if(jwtToken is null)
            {
                logDto.Error += "Could not create JwtToken!";
                _messageBusClient.PublishNewLog(logDto);

                return StatusCode(StatusCodes.Status503ServiceUnavailable, DateTime.Now);   
            }

            //assumption: ** There is no need to create new refresh token when there is active one in the database **
            //Get latest active refresh token from a database for a user with given email
            Console.WriteLine($"[CreateToken] Retrieving active refresh token if exists...");
            var activeRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(jwtTokenCreateRequestDto.Email);
            
            

            if(activeRefreshToken is not null && activeRefreshToken.DeactivationDate > DateTime.Now)
            {
                logDto.Message = "Active refresh token exists. Returning it instead of creating a new one.";
                _messageBusClient.PublishNewLog(logDto);

                return Ok(new JwtTokenCreateResponseDto() 
                    {
                        JwtToken = jwtToken,
                        RefreshToken = activeRefreshToken.Token 
                    });
            }

            Console.WriteLine($"[CreateToken] Token is null or has expired.");

            Console.WriteLine($"[CreateToken]Creating new refresh token...");
            var newRefreshToken = _tokenService.CreateRefreshToken(jwtTokenCreateRequestDto.Id);

            //We do not store Jwt token in the database (its 'decoded' on demand with secret key) but we store our refresh token since its just a random generated string
            var result = await _refreshTokenRepository.SaveRefreshTokenAsync(newRefreshToken);
            Console.WriteLine($"[CreateToken] RefreshToken saved to the repository: {result}");
            
            if(result < 0)
            {
                logDto.Error = "Could not save Refresh Token in the Database!";
                _messageBusClient.PublishNewLog(logDto);

                return StatusCode(StatusCodes.Status503ServiceUnavailable, DateTime.Now);   
            }

            logDto.Message = "Tokens created succesfully.";
            _messageBusClient.PublishNewLog(logDto);

            //We are done. Return both tokens
            return Ok(new JwtTokenCreateResponseDto() 
                {
                    JwtToken = jwtToken,
                    RefreshToken = newRefreshToken.Token
                });
        }
        
        [HttpPost]
        [Route("/api/refresh")]
        public async Task<IActionResult> RefreshTokensAsync([FromBody] RefreshTokenRequest refreshTokenIDto)
        {
            var logDto = new LogRequestDto(){ Date = DateTime.Now, ServiceName = "JwtService", UserId = refreshTokenIDto.UserId, Method = "RefreshTokens", Message = $"Refresh tokens for user {refreshTokenIDto.UserId}."};
            Console.WriteLine($"--START--> [RefreshTokens]");

            var jwtValidationResult = await _tokenService.ValidateExpiredJwtTokenAsync(refreshTokenIDto.JwtToken);
            Console.WriteLine($"--> ValidateJwtTokenResult: {jwtValidationResult}");

            if(!jwtValidationResult)
            {
                Console.WriteLine($"--> ValidateJwtTokenResult: Could not validate tokens.");

                logDto.Error = "Could not validate JwtToken!";
                _messageBusClient.PublishNewLog(logDto);
                
                return BadRequest();
                //throw new SecurityTokenException("JwtToken validation failed!");
            }

            var dbRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshTokenIDto.UserEmail);

            if(dbRefreshToken is null)
            {
                logDto.Error = "Refresh token could not be found in the database";
                _messageBusClient.PublishNewLog(logDto);

                throw new SecurityTokenException("Refresh Token not found!");
            }
            else if (dbRefreshToken.DeactivationDate < DateTime.Now)
            {
                //Refresh token expired!
            }

            //Check if provided token is the same as the one stored in the database.
            if(!refreshTokenIDto.RefreshToken.Equals(dbRefreshToken.Token))
            {
                logDto.Error = "Invalid refresh token passed!";
                _messageBusClient.PublishNewLog(logDto);

                throw new SecurityTokenException("Invalid refresh token passed!");
            }
            
            //So far we know that we have valid Jwt token and provided refresh token is the same as the one in database
            //We can create new tokens now.

            //Get claims from expired token that we got
            var expiredTokenClaimsPrincipal = await _tokenService.GetClaimsPrincipalFromJwtTokenAsync(refreshTokenIDto.JwtToken);
            var expiredTokenClaims = expiredTokenClaimsPrincipal.Claims;
            
            //Create new Jwt Token
            var createdJwtToken = await _tokenService.CreateJwtTokenAsync(expiredTokenClaims);

            //Generate new refresh token.
            var refreshToken = _tokenService.CreateRefreshToken(refreshTokenIDto.UserId);
            Console.WriteLine($"[CreateToken] New RefreshToken should be created. RefreshToken: {refreshToken.Token}");

            Console.WriteLine($"User id: Dto:{refreshTokenIDto.UserId} Database:{dbRefreshToken.UserId}");
            if(refreshTokenIDto.UserId != dbRefreshToken.UserId)
            {
                logDto.Error = "Refresh tokens do not match. Users id mismatch.";
                _messageBusClient.PublishNewLog(logDto);

                throw new SecurityTokenException("Tokens user id mismatch!");
            }

            //We should have jwtToken and refresh token created. Remove old refresh token from the database

            //NOTICE: We may be using old refresh token. It is when refresh token has not been used and it is reused instead of being created. 
            await _refreshTokenRepository.RemoveRefreshTokenAsync(dbRefreshToken.RId, dbRefreshToken.UserId);

            //Save new token to the database.
            var result = await _refreshTokenRepository.SaveRefreshTokenAsync(refreshToken);
            
            logDto.Message = $"Tokens succesfully refreshed.";
            _messageBusClient.PublishNewLog(logDto);

            Console.WriteLine($"--END--> [RefreshToken]");
            return Ok(new JwtTokenRefreshResponseDto()
                {
                    JwtToken = createdJwtToken,
                    RefreshToken = refreshToken.Token
                });
        }
    }
}