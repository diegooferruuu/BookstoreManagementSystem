using System.ComponentModel.DataAnnotations;

namespace ServiceProducts.Domain.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public Guid CategoryId { get; set; }
        
        [Required(ErrorMessage = "El precio es obligatorio.")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "El stock es obligatorio.")]
        public int Stock { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }
    }
}
