using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NutritionWebClient.SyncDataService.Login;
using NutritionWebClient.TokenStorageService;

namespace NutritionWebClient.Services
{
    public class IdentityValidationProvider : RevalidatingServerAuthenticationStateProvider
    {
        private readonly ILoginDataClient _loginDataClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<IdentityOptions> _optionsAccessor;
        private readonly ITokenStorage _tokenStorage;

        public IdentityValidationProvider(ILoggerFactory loggerFactory, ILoginDataClient loginDataClient, IServiceScopeFactory scopeFactory, IOptions<IdentityOptions> optionsAccessor, ITokenStorage tokenStorage)
            : base(loggerFactory)
        {
            _loginDataClient = loginDataClient;
            _scopeFactory = scopeFactory;
            _optionsAccessor = optionsAccessor;
            _tokenStorage = tokenStorage;
        }
        
        protected override TimeSpan RevalidationInterval => TimeSpan.FromSeconds(30);

        protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Authentication state provider checking userauthentication. {DateTime.Now}");
            Console.WriteLine($"Authentication state user: {authenticationState.User.Identity.IsAuthenticated}");
            
            var jwt = _tokenStorage.FetchToken("jwt");
            var refresh = _tokenStorage.FetchToken("refresh");

            Console.WriteLine($"Authentication state jwt: {jwt}");
            Console.WriteLine($"Authentication state refresh: {refresh}");

            if(string.IsNullOrEmpty(jwt) || string.IsNullOrEmpty(refresh))
            {
                return false;
            }

            var userIdClaim = authenticationState.User.Claims.First(c=>c.Type.Equals("userid"));
            var userId = int.Parse(userIdClaim.Value);

            var securityStampClaim = authenticationState.User.Claims.First(c=>c.Type.Equals("secstamp"));
            var localSecurityStamp = securityStampClaim.Value;

            var userSecurityStamp = await _loginDataClient.GetUserSecurityStamp(new Dtos.UserSecurityStampRequest() { UserId = userId });

            Console.WriteLine($"LocalSecurityStamp: {localSecurityStamp}, DatabaseSecurityStamp: {userSecurityStamp.SecKey}");

            if(localSecurityStamp.Equals(userSecurityStamp.SecKey))
                return true;

            return false;
        }
    }
}