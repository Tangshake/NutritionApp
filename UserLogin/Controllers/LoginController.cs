using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UserLogin.AsyncDataServices.RabbitMQ;
using UserLogin.Dtos;
using UserLogin.Dtos.RabbitMq;
using UserLogin.Repository;
using UserLogin.SyncDataService;

namespace UserLogin.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtDataClient _jwtDataClient;
        private readonly IConfiguration _configuration;
        private readonly IMessageBusClient _messageBusClient;

        public LoginController(IUserRepository userRepository, IJwtDataClient jwtDataClient, IMessageBusClient messageBusClient, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtDataClient = jwtDataClient;
            _configuration = configuration;
            _messageBusClient = messageBusClient;
        }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokensResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("/api/login")]
        public async Task<ActionResult> LoginUserAsync([FromBody]UserLoginDto userLoginDto)
        {
            if(userLoginDto is null)
                return BadRequest();

            Console.WriteLine($"--> [Login] Received request to login user with email: {userLoginDto.Email}");
            var logDto = new LogDto(){ Date = DateTime.Now, ServiceName = "UserLogin", Method = "LoginUserAsync", Message = $"Login User."};
            
            //Get user from repository
            var user = await _userRepository.GetUserAsync(userLoginDto.Email);

            if(user != null && user.Activated)
            {
                //Compare passwords
                var passwordVerResult = PasswordManagement.PasswordHash.VerifyHashedPassword(user.PasswordHash, userLoginDto.Password);

                if(passwordVerResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
                {
                    //Generate JWTToken with user claims and refresh token.
                    var tokens = await _jwtDataClient.GetJwtTSecurityTokenAsync(user.AsDto(), _configuration.GetSection("KnownServices:JwtTokenService").Value);

                    //Create Security Stamp
                    var securtiyStampGuid = Guid.NewGuid().ToString();
                    var rowsAffected = _userRepository.CreateSecurityStampForUserAsync(user.Id, securtiyStampGuid);
                    Console.WriteLine($"--> [Login] Created SecurityStamp: {securtiyStampGuid}");
                    
                    //Deserialize object and add 2 more values
                    var tokensResponseDto = JsonSerializer.Deserialize<TokensResponseDto>(tokens);
                    tokensResponseDto.UserId = user.Id;
                    tokensResponseDto.SecurityStamp = securtiyStampGuid;

                    logDto.Message = $"User {user.Id} logged in succesfully!";
                    logDto.UserId = user.Id;
                    _messageBusClient.PublishNewLog(logDto);

                    return Ok(tokensResponseDto);
                }
                else
                {
                    logDto.Error = $"User {user.Id} login password verification failed!";
                    _messageBusClient.PublishNewLog(logDto);
                    
                    return BadRequest();
                }
            }
            else
            {
                logDto.Error = "User login data corrupted.";
                _messageBusClient.PublishNewLog(logDto);

                return BadRequest();
            }
        }
        
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokensResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("/api/secstamp")]
        public async Task<ActionResult> GetSecurityStamp([FromBody] SecurityStampRequest securityStampRequest)
        {
            Console.WriteLine($"--> [GetSecurityStamp] User: {securityStampRequest.UserId}");
            var logDto = new LogDto(){ Date = DateTime.Now, ServiceName = "UserLogin", Method = "GetSecurityStamp", Message = $"Get Security Stamp."};

            //Get user from repository
            var userSecurityStamp = await _userRepository.GetUserSecurityStampAsync(securityStampRequest.UserId);

            if(userSecurityStamp != null)
            {
                logDto.Error = "";
                logDto.Message = "Retrieving user security stamp successfull";
                logDto.UserId = securityStampRequest.UserId;
                _messageBusClient.PublishNewLog(logDto);

                return Ok(userSecurityStamp);
            }
            else
            {
                logDto.Error = "Retrieving user security stamp failed!";
                logDto.UserId = securityStampRequest.UserId;
                _messageBusClient.PublishNewLog(logDto);

                return BadRequest();
            }
        }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("api/secstampupdate")]
        public async Task<ActionResult> UpdateSecurityStamp([FromBody] SecurityStampUpdateRequestDto securityStampUpdateRequestDto)
        {
            if(securityStampUpdateRequestDto is null)
                return BadRequest();
            
            Console.WriteLine($"--> [UpdateSecurityStamp] User: {securityStampUpdateRequestDto.UserId}");

            var logDto = new LogDto(){ Date = DateTime.Now, ServiceName = "UserLogin", Method = "UpdateSecurityStamp", Message = $"Update Security Stamp."};
            logDto.UserId = securityStampUpdateRequestDto.UserId;
            _messageBusClient.PublishNewLog(logDto);

            //Get user from repository
            await _userRepository.UpdateUserSecurityTimestampAsync(securityStampUpdateRequestDto.UserId, securityStampUpdateRequestDto.SecurityStamp);

            return Ok();
        }

        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("/api/refresh")]
        public async Task<ActionResult> RefreshTokenAsync([FromBody]RefreshTokenDto refreshTokenDto)
        {
            Console.WriteLine($"--> [Login] Received request to refresh token for user with email: {refreshTokenDto.UserEmail} id:{refreshTokenDto.UserId}");
            var logDto = new LogDto(){ Date = DateTime.Now, ServiceName = "UserLogin", Method = "RefreshTokenAsync", Message = $"Refresh Jwt Token."};
            logDto.UserId = refreshTokenDto.UserId;

            //Get user from repository
            var user = await _userRepository.GetUserAsync(refreshTokenDto.UserEmail);

            if(user != null && user.Activated)
            {
                //Refresh JWTToken.
                var token = await _jwtDataClient.RefreshJwtTSecurityTokenAsync(refreshTokenDto, _configuration.GetSection("KnownServices:JwtTokenRefreshService").Value);

                //Console.WriteLine($"--> [Login] Received tokens. JwtToken: {tokens.JwtToken} RefreshToken: {tokens.RefreshToken}");
                
                logDto.Error = "Ok";
                logDto.UserId = refreshTokenDto.UserId;
                _messageBusClient.PublishNewLog(logDto);

                return Ok(token);
            }
            else
            {
                logDto.Error = "Failed to refresh Jwt Token!";
                logDto.UserId = refreshTokenDto.UserId;
                _messageBusClient.PublishNewLog(logDto);
                
                return BadRequest();
            }
        }
    }
}