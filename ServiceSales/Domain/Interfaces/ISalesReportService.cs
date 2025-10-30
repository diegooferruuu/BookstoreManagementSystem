using ServiceSales.Domain.Models;

namespace ServiceSales.Domain.Interfaces
{
    public interface ISalesReportService
    {
        Task<byte[]> GenerateSalesReportAsync(SaleReportFilter filter, string reportType, string createdBy = "Sistema", CancellationToken ct = default);
        Task<string> GetReportContentType(string reportType);
        Task<string> GetReportFileExtension(string reportType);
    }
}
