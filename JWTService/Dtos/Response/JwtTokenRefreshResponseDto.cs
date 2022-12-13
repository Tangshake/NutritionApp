namespace JWTService.Dtos.Response
{
    public class JwtTokenRefreshResponseDto
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }

    }
}