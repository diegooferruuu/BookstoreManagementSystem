using System.ComponentModel.DataAnnotations;

namespace BookstoreManagementSystem.Domain.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required(ErrorMessage = "La categoria es obligatoria.")]
        public Guid Category_id { get; set; }
        [Required(ErrorMessage = "El precio es obligatorio.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "El stock es obligatorio.")]
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Propiedad adicional para mostrar el nombre de la categoría en las vistas
        public string? CategoryName { get; set; }
    }
}
