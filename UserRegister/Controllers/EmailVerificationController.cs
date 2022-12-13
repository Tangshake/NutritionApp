using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using NETCore.MailKit.Core;
using UserRegister.Dtos;
using UserRegister.Repository;

namespace UserRegister.Controller
{
    [ApiController]
    [Route("/[controller]")]
    public class EmailVerificationController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IActivationRepository _activation;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment  _hostingEnvironment;

        JsonSerializerOptions options = new JsonSerializerOptions {Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };

        public EmailVerificationController(IUserRepository repository, IActivationRepository activation, IEmailService emailService, IWebHostEnvironment hostingEnvironment)
        {
            _repository = repository;
            _activation = activation;
            _emailService = emailService;
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("/api/verifyemail")]
        public async Task<ActionResult> VerifyEmailAsync(int userId, string token)
        {
            Console.WriteLine("--> [VerifyEmail]");

            var verificationStatusDto = new VerificationStatusDto(){ ErrorCode = -1, Message = "Verification failed." };
            Console.WriteLine($"--> User with id {userId} want to verify its email.");
            
            //Chack if users account is activated. If soooo ...  skip the activation
            var activated = await _repository.UserActivatedAsync(userId);
            Console.WriteLine($"Is account activated: {activated}");

            if(activated is null || activated == true)
            {
                verificationStatusDto.ErrorCode = 2;
                verificationStatusDto.Message = "Email verified succesfully";
                
                return new ContentResult {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = "<html><head><meta charset=\"utf-8\" /></head><body><div><a href=\"https://localhost:5003\"><button style=\"text-align center; font-size:x-large;\" type=\"submit\">Przejdź do strony logowania.</button></a></div></body></html>"
                };

                //return Ok(JsonSerializer.Serialize(verificationStatusDto, options));
            }

            //Get user with given id and provided token
            var activationUser = await  _activation.GetUserActivation(userId, token);
            
            if(activationUser is not null)
            {
                //Check for expiration time
                var timespan = DateTime.Now.Subtract(activationUser.ActivationStart);

                Console.WriteLine($"User verified its email after: {timespan.TotalSeconds} seconds");

                if(timespan.Seconds >= 3600) // DateTime.Now > activationUser.ActivationExpire
                {
                    Console.WriteLine($"--> [VerifyEmail] User with id {userId} waited for too long. Verification timeout.");
                    return Ok(JsonSerializer.Serialize(verificationStatusDto, options));        
                }
                else
                {
                    if(!token.Equals(activationUser.Token))
                    {
                        Console.WriteLine($"--> [VerifyEmail] User with id {userId} Verification codes do not match. Account cannot be activated.");
                        return Ok(JsonSerializer.Serialize(verificationStatusDto, options));  
                    }

                    var setVerification = await _repository.SetEmailAccountAsVerifiedAsync(userId);
                    if(setVerification == 1)
                    {
                        Console.WriteLine($"--> [VerifyEmail] User with id {userId} verified its account succesfully. Sending that information back.");
                        verificationStatusDto.ErrorCode = 0;
                        verificationStatusDto.Message = "Email verified succesfully";

                        return new ContentResult {
                            ContentType = "text/html",
                            StatusCode = (int)HttpStatusCode.OK,
                            Content = "<html><head><meta charset=\"utf-8\" /></head><body><div><a href=\"https://localhost:5003\"><button style=\"text-align center; font-size:x-large;\" type=\"submit\">Przejdź do strony logowania.</button></a></div></body></html>"
                        };
                        //return Ok(JsonSerializer.Serialize(verificationStatusDto, options));
                    }
                    else
                    {
                        Console.WriteLine($"--> [VerifyEmail] Could not set account as verified for user with id {userId} .");
                        return Ok(JsonSerializer.Serialize(verificationStatusDto, options));
                    }
                }
            }
            
            return Ok(JsonSerializer.Serialize(verificationStatusDto, options));
        }
    }
}