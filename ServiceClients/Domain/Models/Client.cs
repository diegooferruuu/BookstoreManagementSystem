using System.ComponentModel.DataAnnotations;

namespace ServiceClients.Domain.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string LastName { get; set; } = string.Empty;
        
        public string? MiddleName { get; set; }
        
        [Required(ErrorMessage = "El correo es obligatorio.")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "El telefono es obligatorio.")]
        public string? Phone { get; set; }
        
        [Required(ErrorMessage = "La direccion es obligatoria.")]
        public string? Address { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}
