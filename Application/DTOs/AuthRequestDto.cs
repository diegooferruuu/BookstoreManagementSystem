using System.ComponentModel.DataAnnotations;

namespace BookstoreManagementSystem.Application.DTOs
{
    public class AuthRequestDto
    {
        [Required]
        [MinLength(1)]
        public string UserOrEmail { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
