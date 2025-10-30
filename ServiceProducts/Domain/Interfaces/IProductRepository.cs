using ServiceProducts.Domain.Models;
using ServiceProducts.Domain.Reports;

namespace ServiceProducts.Domain.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        Product? Read(Guid id);
        void Create(Product product);
        void Update(Product product);
        void Delete(Guid id);
        Task<IReadOnlyList<ProductReportRow>> GetForReportAsync(decimal? priceMin, decimal? priceMax, Guid? categoryId, CancellationToken ct);
    }
}
