using System.ComponentModel.DataAnnotations;

namespace ServiceUsers.Application.DTOs
{
    public class AuthRequestDto
    {
        [Required(ErrorMessage = "El campo Usuario o Correo es obligatorio.")]
        [Display(Name = "Usuario o Correo")]
        public string UserOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contrase�a es obligatoria.")]
        [Display(Name = "Contrase�a")]
        public string Password { get; set; } = string.Empty;
    }
}
