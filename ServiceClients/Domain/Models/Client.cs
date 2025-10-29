using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceClients.Domain.Models
{
    public class Client
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[A-Za-z������������]+$", ErrorMessage = "El nombre solo puede contener letras, sin espacios.")]
        [StringLength(50, ErrorMessage = "El nombre no debe superar los 50 caracteres.")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [RegularExpression(@"^[A-Za-z������������\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios.")]
        [StringLength(100, ErrorMessage = "El apellido no debe superar los 100 caracteres.")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Correo electr�nico")]
        [Required(ErrorMessage = "El correo electr�nico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electr�nico v�lido.")]
        [StringLength(150, ErrorMessage = "El correo no debe superar los 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Tel�fono")]
        [Required(ErrorMessage = "El n�mero de tel�fono es obligatorio.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El n�mero de tel�fono debe tener exactamente 8 d�gitos.")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Direcci�n")]
        [Required(ErrorMessage = "La direcci�n es obligatoria.")]
        [StringLength(200, ErrorMessage = "La direcci�n no debe superar los 200 caracteres.")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Fecha de creaci�n")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
