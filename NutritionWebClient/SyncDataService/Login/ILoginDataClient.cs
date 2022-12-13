using System.Threading.Tasks;
using NutritionWebClient.Dtos;
using NutritionWebClient.Dtos.RefreshToken;
using NutritionWebClient.Dtos.User;
using NutritionWebClient.Model;

namespace NutritionWebClient.SyncDataService.Login
{
    public interface ILoginDataClient
    {
        Task<RegisterResponseDto> SendUserRegistrationData(UserRegisterDto user);

        Task<EmailVerificationResponseDto> SendEmailVerificationData(EmailVerificationDto user);

        Task<string> SendLoginUserData(UserLoginDto user);

        Task<string> RefreshJwtTokenAsync(RefreshTokenDto refreshTokenDto);

        Task<UserSecurityStampResponse> GetUserSecurityStamp(UserSecurityStampRequest userSecurityStampRequest);

        Task UpdateUserSecurityStamp(UpdateUserSecurityStampRequest updateSecurityStampRequest);

    }
}