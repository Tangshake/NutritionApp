using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NutritionWebClient.Dtos.Products;
using static NutritionWebClient.Model.Views;

namespace NutritionWebClient.Pages.MainView
{
    public partial class Index : ComponentBase
    {
        string token;
        DateTime? dateExpiresOn;

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask {get;set;}

        [Parameter]
        public ProductReadDto productReadDto { get; set; }

        [Parameter]
        public string SearchString { get; set; }

        [Parameter]
        public ViewToShow ShowView {get;set;} = ViewToShow.NoView;

        public string UserEmail {get;set;}
        public int UserId { get; set; }

        public void OnViewChangeRequest(ViewToShow view)
        {
            Console.WriteLine($"[ViewChange] {view}");
            ShowView = view;
        }

        protected override async Task OnInitializedAsync()
        {
            token = _tokenStorage.FetchToken("jwt");
            var exp = JwtTokenHelper.JwtHelper.ExctractClaim(token, "exp");
            UserEmail = JwtTokenHelper.JwtHelper.ExctractClaim(token, ClaimTypes.Email);
            
            if(int.TryParse(JwtTokenHelper.JwtHelper.ExctractClaim(token, "id"), out int id))
                UserId = id;

            if(!string.IsNullOrEmpty(exp))
                dateExpiresOn = DateTimeOffset.FromUnixTimeSeconds(int.Parse(exp)).LocalDateTime;

            var authState = await AuthenticationStateTask;
            Console.WriteLine($"Is User authenticated: {authState.User.Identity.IsAuthenticated}");
            foreach(var claim in authState.User.Claims)
                Console.WriteLine($"claim: {claim.Type.ToString()} value: {claim.Value.ToString()}");
        }

        //TODO: USER Identity Hardcoded!!
        private async Task Refresh()
        {
            Console.WriteLine($"Refresh pressed: {DateTime.Now}");
            var jwtTokenExpired = JwtTokenHelper.JwtHelper.Expired(_tokenStorage.FetchToken("jwt"));

            var authState = await AuthenticationStateTask;
            Console.WriteLine($"Is User authenticated: {authState.User.Identity.IsAuthenticated}");
            foreach(var claim in authState.User.Claims)
                Console.WriteLine($"claim: {claim.Type.ToString()} value: {claim.Value.ToString()}");

            Console.WriteLine($"--> Has JwtToken expired?: {jwtTokenExpired}");
            if(jwtTokenExpired)
            {
                var jwtString = _tokenStorage.FetchToken("jwt");
                var refreshString = _tokenStorage.FetchToken("refresh");
                var refreshJwtTokenResult = await _loginDataClient.RefreshJwtTokenAsync(new Dtos.RefreshToken.RefreshTokenDto(){ JwtToken = jwtString, RefreshToken = refreshString, UserEmail = "1", UserId = 112 });

                _tokenStorage.RemoveToken("jwt");
                _tokenStorage.RemoveToken("refresh");

                var tokensReadDto = JsonSerializer.Deserialize<TokensReadDto>(refreshJwtTokenResult);
                Console.WriteLine($"--> [Refresh] JwtTokenString: {tokensReadDto.JwtToken}");
                Console.WriteLine($"--> [Refresh] RefreshTokenString: {tokensReadDto.RefreshToken}");

                _tokenStorage.AddToken("jwt", tokensReadDto.JwtToken);
                _tokenStorage.AddToken("refresh", tokensReadDto.RefreshToken);

                Console.WriteLine($"Tokens added to cache");
                
                var token = _tokenStorage.FetchToken("jwt");
                var exp = JwtTokenHelper.JwtHelper.ExctractClaim(token, "exp");
                if(!string.IsNullOrEmpty(exp))
                    dateExpiresOn = DateTimeOffset.FromUnixTimeSeconds(int.Parse(exp)).LocalDateTime;
            }

            //productReadDto = await _productDataClient.GetProductByIdAsync(1,"sdsd");
        }
    }
}