using ServiceProducts.Domain.Reports;

namespace ServiceProducts.Domain.Interfaces.Reports;

public interface IReportDirector
{
    ReportResult Make(ProductReportData data, IReportBuilder builder, byte[]? logoBytes, string fileName);
}
