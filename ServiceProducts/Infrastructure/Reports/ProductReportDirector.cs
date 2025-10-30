using ServiceProducts.Domain.Interfaces.Reports;
using ServiceProducts.Domain.Reports;

namespace ServiceProducts.Infrastructure.Reports;

public sealed class ProductReportDirector : IReportDirector
{
    public ReportResult Make(ProductReportData data, IReportBuilder builder, byte[]? logoBytes, string fileName)
    {
        builder.Reset();
        builder.SetHeader(data.Title, data.GeneratedBy, data.GeneratedAt, logoBytes);
        builder.SetBody(data);
        builder.SetFooter($"Reporte Generado: {data.GeneratedBy} - {data.GeneratedAt:dd/MM/yyyy HH:mm:ss}");
        return builder.Build(fileName);
    }
}
