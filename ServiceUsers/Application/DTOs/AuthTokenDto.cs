namespace ServiceUsers.Application.DTOs
{
    public class AuthTokenDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
    }
}
