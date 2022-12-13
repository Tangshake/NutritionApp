using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using NETCore.MailKit.Core;
using UserRegister.Dtos;
using UserRegister.PasswordManagement;
using UserRegister.Repository;

namespace UserRegister.Controller
{
    [ApiController]
    [Route("/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;
        private readonly IActivationRepository _activation;
        private readonly IEmailService _emailService;
        JsonSerializerOptions options = new JsonSerializerOptions {Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };

        public RegisterController(IConfiguration configuration, IUserRepository repository, IActivationRepository activation, IEmailService emailService)
        {
            _configuration = configuration;
            _repository = repository;
            _activation = activation;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("/api/register")]
        public async Task<ActionResult> RegisterUserAsync([FromBody] RegisterDto registerDto)
        {
            Console.WriteLine($"--> [RegisterUser] Got request to register user: {registerDto.Login} ");
            //-1: unexpected error
            // 0: does not exist
            // 1: exists
            var exist = await _repository.UserExistsAsync(registerDto.Login, registerDto.Email);
            Console.WriteLine($"[UserRegisterAsync] User exists: {exist}");

            if(exist == 0)
            {
                //User does not exist. Create a new one with provided login and password.
                //Hash provided password
                var hashedPassword = PasswordManagement.PasswordHash.HashPassword(registerDto.Password, RandomNumberGenerator.Create());
                
                //Create user
                var user_id = await _repository.CreateUserAsync(registerDto.Login, "regular", registerDto.Email, Convert.ToBase64String(hashedPassword));
                Console.WriteLine($"[UserRegisterAsync] New User! Id: {user_id}");

                if(user_id != -1)
                {
                    //Account has been created. 
                    //Generate email confirmation token.
                    var token = EmailTokenGenerator.GenerateToken(128);
                    var tokenString = Convert.ToBase64String(token);

                    //Insert email verification entry
                    var result = await _repository.CreateEmailVerificationEntryAsync(user_id, tokenString, DateTime.Now, DateTime.Now.AddHours(1));
                    
                    if(result == 1)
                    {
                        //Send email verification link. Generate link.
                        //alternative: var link = Url.Action(nameof(VerifyEmail), "Register", new { userId = user_id, token = tokenString}, Request.Scheme, Request.Host.ToString());
                        var query = new Dictionary<string, string>()
                        {
                            ["userId"] = user_id.ToString(),
                            ["token"] = tokenString
                        };

                        var address = _configuration["Register:VerificationAddress"];
                        var uri = QueryHelpers.AddQueryString(address, query);
                        
                        //Send email with confirmation link.
                        //alternative: await _emailService.SendAsync("test@test.com","Nutrition verifying link.", $"<a href=\"{link}\">Verify email link.</a>", true);
                        Console.WriteLine($"[RegisterUserAsync] Verification email link: {uri}");

                        await _emailService.SendAsync("test@test.com","Nutrition verifying link.", $"<a href=\"{uri}\">Verify email link.</a> </ br> {uri}", true);

                        var registerStatusDto = new RegisterStatusDto() { ErrorCode = 0, VerificationEmailSent = true, AccountCreated = true, Message = "Account has been created!. Verification email has been sent." };
                        return Ok(JsonSerializer.Serialize(registerStatusDto, options));
                    }
                    else
                    {
                        var registerStatusDto = new RegisterStatusDto() { ErrorCode = 1, VerificationEmailSent = false, AccountCreated = true, Message = "Unexpected error. Could not create email verification token. Try again later."};
                        return Ok(JsonSerializer.Serialize(registerStatusDto, options));
                    }
                }
                else
                {
                    var registerStatusDto = new RegisterStatusDto() {ErrorCode = 1, VerificationEmailSent = false, AccountCreated = false, Message = "Could not create account. Try again later." };
                    return Ok(JsonSerializer.Serialize(registerStatusDto, options));
                }
            }
            else if(exist > 0)
            {
                var registerStatusDto = new RegisterStatusDto() {ErrorCode = 2, VerificationEmailSent = false, AccountCreated = false, Message = "Account already exists." };
                return Ok(JsonSerializer.Serialize(registerStatusDto, options));
            }
            else
            {
                var registerStatusDto = new RegisterStatusDto() { ErrorCode = -1, VerificationEmailSent = false, AccountCreated = false, Message = "Unexpected error. Could not register user. Try again later." };
                return Ok(JsonSerializer.Serialize(registerStatusDto, options)); 
            }
        }
    }     
}