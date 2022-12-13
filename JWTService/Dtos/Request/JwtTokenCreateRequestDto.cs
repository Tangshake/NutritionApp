namespace JWTService.Dtos.Request
{
    public class JwtTokenCreateRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}