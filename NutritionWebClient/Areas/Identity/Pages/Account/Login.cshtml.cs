using System;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NutritionWebClient.Dtos.User;
using NutritionWebClient.Model;
using NutritionWebClient.SyncDataService.Login;
using NutritionWebClient.TokenStorageService;

namespace NutritionWebClient.Areas.Identity.Pages.Account
{
    public class LoginPageModel : PageModel
    {
        private readonly ILoginDataClient _loginDataClient;
        private readonly ITokenStorage _tokenStorage;
        private readonly IWebHostEnvironment _hostingEnvironment;

        [BindProperty]
        public RegisterModel RegisterModel {get; set;}
        
        [BindProperty]
        public LoginModel LoginModel {get; set;}
        
        public LoginPageModel(ILoginDataClient loginDataClient, ITokenStorage tokenStorage, IWebHostEnvironment hostingEnvironment)
        {
            _loginDataClient = loginDataClient;
            _tokenStorage = tokenStorage;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> OnPostRegister(RegisterModel registerModel)
        {
            
            if(registerModel != null && !string.IsNullOrEmpty(registerModel.Password) && !string.IsNullOrEmpty(registerModel.Email) && registerModel.Password.Equals(registerModel.RepeatedPassword))
            {
                Console.WriteLine($"--> [WebClient:Send] Register user data send to register webservice: {registerModel.Email} {registerModel.Login} ");
                var registerModelRequestDto = registerModel.AsDto();
                var registerResponseDto = await _loginDataClient.SendUserRegistrationData(registerModelRequestDto);
                
                registerModel = new();  //Clear 

                Console.WriteLine($"--> [WebClient(Register):Response] Received message: {registerResponseDto.Message} {registerResponseDto.VerificationEmailSent}");

                if(registerResponseDto.VerificationEmailSent)
                {
                    var path = Path.Combine(_hostingEnvironment.WebRootPath, "template", "template.html");
                    var html = System.IO.File.ReadAllText(path);
                    
                    html = html.Replace("{{userName}}", registerModelRequestDto.Login);
                    html = html.Replace("{{email}}", registerModelRequestDto.Email);

                    return new ContentResult() 
                    {
                        Content = html,
                        ContentType = "text/html"
                    };
                }
            }
            else
            {
                Console.WriteLine($"--> [WebClient(Register)] Registering went wrong.");
            }

            return LocalRedirect("/");
        }

        public async Task<IActionResult> OnPostLogin(LoginModel loginModel)
        {
            Console.WriteLine($"--> OnPostLogin");
            if(loginModel != null && !string.IsNullOrEmpty(loginModel.Password) && !string.IsNullOrEmpty(loginModel.Email) && loginModel.Password.Equals(loginModel.RepeatedPassword))
            {
                Console.WriteLine($"--> [OnPostLogin:Send] Login user data send to login webservice.");
                var loginResultString = await _loginDataClient.SendLoginUserData(loginModel.AsDto());
                Console.WriteLine($"--> [OnPostLogin(Login):Repsponse] Received message: {loginResultString}");

                LoginModel = new();  //Clear the form

                if(!string.IsNullOrEmpty(loginResultString))
                {
                    var tokensReadDto = JsonSerializer.Deserialize<TokensReadDto>(loginResultString);
                    Console.WriteLine($"--> [OnPostLogin:Response] UserId: {tokensReadDto.UserId}");
                    Console.WriteLine($"--> [OnPostLogin:Response] UserSecurityStamp: {tokensReadDto.SecurityStamp}");
                    Console.WriteLine($"--> [OnPostLogin:Response] JwtTokenString: {tokensReadDto.JwtToken}");
                    Console.WriteLine($"--> [OnPostLogin:Response] RefreshTokenString: {tokensReadDto.RefreshToken}");

                    //Add Token to IMemoryCache
                    _tokenStorage.AddToken("jwt", tokensReadDto.JwtToken);
                    _tokenStorage.AddToken("refresh", tokensReadDto.RefreshToken);

                    //CreateUserClaims
                    var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    claimsIdentity.AddClaim(new Claim("email",loginModel.Email));
                    claimsIdentity.AddClaim(new Claim("userid",tokensReadDto.UserId.ToString()));

                    var userRole = JwtTokenHelper.JwtHelper.ExctractClaim(tokensReadDto.JwtToken, ClaimTypes.Role);
                    claimsIdentity.AddClaim(new Claim("secstamp",tokensReadDto.SecurityStamp));
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, userRole));
                    claimsIdentity.AddClaim(new Claim("Role", userRole));

                    //Cookie register
                    Console.WriteLine($"--> [OnPostLogin] Creating cookie.");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                    return LocalRedirect("/");
                }
                else
                {
                    return LocalRedirect("/");
                }
            }
            else
            {
                Console.WriteLine($"--> [OnPostLogin] Login went wrong.");

                return BadRequest();
            }
        }
    }

}