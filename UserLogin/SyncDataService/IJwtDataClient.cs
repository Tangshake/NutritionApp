using System.Threading.Tasks;
using UserLogin.Dtos;

namespace UserLogin.SyncDataService
{
    public interface IJwtDataClient
    {
        Task<string> GetJwtTSecurityTokenAsync(UserCreditentialsDto userCreditentialsDto, string requestUri);
        Task<string> RefreshJwtTSecurityTokenAsync(RefreshTokenDto efreshTokenDto, string requestUri);
    }
}