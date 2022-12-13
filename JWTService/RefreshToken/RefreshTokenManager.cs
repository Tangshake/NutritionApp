using System;
using System.Security.Cryptography;
using JWTService.Entity;

namespace JWTService.RefreshToken
{
    public class RefreshTokenManager : IRefreshTokenManager
    {
        public string Generate(int size)
        {
            byte[] byteArray = new byte[size];
            var rng = RandomNumberGenerator.Create();

            rng.GetBytes(byteArray);

            return Convert.ToBase64String(byteArray);
        }
    }
}