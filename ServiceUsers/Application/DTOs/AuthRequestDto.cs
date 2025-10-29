namespace ServiceUsers.Application.DTOs
{
    public class AuthRequestDto
    {
        public string UserOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
