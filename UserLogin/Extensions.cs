using UserLogin.Dtos;
using UserLogin.Entity;

namespace UserLogin
{
    public static class Extensions
    {
        public static UserCreditentialsDto AsDto(this User user)
        {
            return new UserCreditentialsDto()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}