using System.ComponentModel.DataAnnotations;

namespace ServiceSales.Domain.Models
{
    public class SaleDetail
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid SaleId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public string? ProductName { get; set; }
    }
}
