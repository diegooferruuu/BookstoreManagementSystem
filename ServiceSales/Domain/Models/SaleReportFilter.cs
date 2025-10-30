namespace ServiceSales.Domain.Models
{
    public class SaleReportFilter
    {
        public Guid? UserId { get; set; }
        public Guid? ClientId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
