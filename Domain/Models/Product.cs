namespace BookstoreManagementSystem.Domain.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Category_id { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Propiedad adicional para mostrar el nombre de la categoría en las vistas
        public string? CategoryName { get; set; }
    }
}
