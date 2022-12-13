using System;
using System.Threading.Tasks;
using UserRegister.Entity;

namespace UserRegister.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserByLoginAsync(string name, string email);

        Task<int> UserExistsAsync(string name, string email);

        Task<bool?> UserActivatedAsync(int userId);

        Task<int> CreateUserAsync(string login, string role, string email, string password);

        Task<int> CreateEmailVerificationEntryAsync(int userId, string token, DateTime tokenGenerationDate, DateTime tokenExpirationDate);

        Task<int> SetEmailAccountAsVerifiedAsync(int userId);

    }
}