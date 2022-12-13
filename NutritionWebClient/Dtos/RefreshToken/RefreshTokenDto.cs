namespace NutritionWebClient.Dtos.RefreshToken
{
    public class RefreshTokenDto
    {
        public int UserId { get; set; }
        
        public string UserEmail { get; set; }
        
        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}