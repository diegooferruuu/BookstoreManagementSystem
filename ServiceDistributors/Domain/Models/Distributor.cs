using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceDistributors.Domain.Models
{
    public class Distributor
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe superar los 100 caracteres.")]
        [RegularExpression(@"^[A-Za-z������������0-9\s\.,&\-]+$", ErrorMessage = "El nombre contiene caracteres inv�lidos.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Correo electr�nico")]
        [Required(ErrorMessage = "El correo electr�nico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electr�nico v�lido.")]
        [StringLength(150, ErrorMessage = "El correo no debe superar los 150 caracteres.")]
        public string ContactEmail { get; set; } = string.Empty;

        [Display(Name = "Tel�fono")]
        [Required(ErrorMessage = "El tel�fono es obligatorio.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El tel�fono debe tener exactamente 8 d�gitos.")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Direcci�n")]
        [Required(ErrorMessage = "La direcci�n es obligatoria.")]
        [StringLength(200, ErrorMessage = "La direcci�n no debe superar los 200 caracteres.")]
        public string Address { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
