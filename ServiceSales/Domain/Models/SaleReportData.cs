namespace ServiceSales.Domain.Models
{
    public class SaleReportData
    {
        public Guid SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
