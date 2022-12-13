namespace JWTService.Dtos.Response
{
    public class JwtTokenCreateResponseDto
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }

    }
}