using ServiceProducts.Application.DTOs;
using ServiceProducts.Domain.Interfaces;
using ServiceProducts.Domain.Interfaces.Reports;
using ServiceProducts.Domain.Reports;

namespace ServiceProducts.Application.Services;

public sealed class ProductReportService : IProductReportService
{
    private readonly IProductRepository _products;
    private readonly IReportDirector _director;

    public ProductReportService(IProductRepository products, IReportDirector director)
    {
        _products = products;
        _director = director;
    }

    public async Task<ReportResult> GenerateAsync(ReportFilterDto filter, string format, string generatedBy, byte[]? logoBytes, CancellationToken ct)
    {
        var rows = await _products.GetForReportAsync(filter.PriceMin, filter.PriceMax, filter.CategoryId, ct);

        var data = new ProductReportData
        {
            Title = "LISTA DE PRODUCTOS",
            GeneratedBy = generatedBy,
            GeneratedAt = DateTimeOffset.Now,
            Rows = rows
        };

        IReportBuilder builder = format.ToLowerInvariant() switch
        {
            "pdf" => new ServiceProducts.Infrastructure.Reports.PdfReportBuilder(),
            "xlsx" => new ServiceProducts.Infrastructure.Reports.ExcelReportBuilder(),
            "excel" => new ServiceProducts.Infrastructure.Reports.ExcelReportBuilder(),
            _ => new ServiceProducts.Infrastructure.Reports.PdfReportBuilder()
        };

        var fileName = $"productos_{DateTime.Now:yyyyMMdd_HHmmss}";
        return _director.Make(data, builder, logoBytes, fileName);
    }
}
