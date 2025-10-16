using System.ComponentModel.DataAnnotations;

namespace BookstoreManagementSystem.Application.DTOs
{
    public class AuthRequestDto
    {
        [Required(ErrorMessage = "El usuario o correo es obligatorio.")]
        [MinLength(1)]
        public string UserOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
