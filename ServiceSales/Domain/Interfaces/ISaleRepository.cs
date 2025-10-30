using ServiceSales.Domain.Models;

namespace ServiceSales.Domain.Interfaces
{
    public interface ISaleRepository
    {
        Task<List<Sale>> GetAllAsync(CancellationToken ct = default);
        Task<List<Sale>> GetFilteredSalesAsync(SaleReportFilter filter, CancellationToken ct = default);
        Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<SaleDetail>> GetSaleDetailsAsync(Guid saleId, CancellationToken ct = default);
        Task<Dictionary<string, decimal>> GetTopProductsSalesAsync(SaleReportFilter filter, CancellationToken ct = default);
        Task<Dictionary<string, decimal>> GetProductRevenueAsync(SaleReportFilter filter, CancellationToken ct = default);
    }
}
