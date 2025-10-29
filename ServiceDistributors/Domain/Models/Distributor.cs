using System.ComponentModel.DataAnnotations;

namespace ServiceDistributors.Domain.Models
{
    public class Distributor
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El correo obligatorio.")]
        public string ContactEmail { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El telefono es obligatorio.")]
        public string Phone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La direccion es obligatoria.")]
        public string Address { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
    }
}
