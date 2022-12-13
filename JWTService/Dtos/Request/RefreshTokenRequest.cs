using System;

namespace JWTService.Dtos.Request
{
    public class RefreshTokenRequest
    {
        public int UserId { get; set; }
        
        public string UserEmail { get; set; }
        
        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}