namespace TurboVolt.DTOs
{
    public class LoginRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }


    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public UserResponseDto User { get; set; } = null!;
    }
}
