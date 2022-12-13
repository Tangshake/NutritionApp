using System.Security.Cryptography;

namespace UserRegister.PasswordManagement
{
    public static class EmailTokenGenerator
    {
        public static byte[] GenerateToken(int size)
        {

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] token = new byte[size];
            rng.GetBytes(token);

            return token;
        }
    }
}