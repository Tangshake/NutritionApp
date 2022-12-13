using System.Threading.Tasks;
using UserLogin.Entity;

namespace UserLogin.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(string email);

        Task<int> CreateSecurityStampForUserAsync(int userId, string guid);

        Task<UserSecurityStamp> GetUserSecurityStampAsync(int userId);

        Task UpdateUserSecurityTimestampAsync(int userId, string guid);
    }
}