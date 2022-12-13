using System.ComponentModel.DataAnnotations;

namespace UserRegister.Dtos
{
    public class RegisterDto
    {
        public string Password { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }
    }
}