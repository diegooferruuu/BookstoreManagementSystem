using ServiceProducts.Application.DTOs;
using ServiceProducts.Domain.Reports;

namespace ServiceProducts.Domain.Interfaces.Reports;

public interface IProductReportService
{
    Task<ReportResult> GenerateAsync(ReportFilterDto filter, string format, string generatedBy, byte[]? logoBytes, CancellationToken ct);
}
