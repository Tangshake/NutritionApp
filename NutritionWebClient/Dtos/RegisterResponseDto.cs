namespace NutritionWebClient.Dtos
{
    public class RegisterResponseDto
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public bool AccountCreated { get; set; }
        public bool VerificationEmailSent { get; set; }
    }
}