using ServiceProducts.Domain.Reports;

namespace ServiceProducts.Domain.Interfaces.Reports;

public interface IReportBuilder
{
    void Reset();
    void SetHeader(string title, string generatedBy, DateTimeOffset generatedAt, byte[]? logoBytes);
    void SetBody(ProductReportData data);
    void SetFooter(string footerText);
    ReportResult Build(string suggestedFileName);
}
