namespace UserLogin.Dtos
{
    public class TokensResponseDto
    {
        public int UserId { get; set; }
        public string SecurityStamp { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }

    }
}