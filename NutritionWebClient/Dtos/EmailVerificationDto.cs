namespace NutritionWebClient.Dtos
{
    public class EmailVerificationDto
    {
        public int UserId { get; set; }

        public string Token { get; set; }
    }
}