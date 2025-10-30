using ServiceCommon.Domain.Interfaces;

namespace ServiceCommon.Application.Services
{
    public enum ReportType
    {
        Pdf,
        Excel
    }

    public class ReportBuilderFactory
    {
        public static IReportBuilder CreateBuilder(ReportType reportType)
        {
            return reportType switch
            {
                ReportType.Pdf => new PdfReportBuilder(),
                ReportType.Excel => new ExcelReportBuilder(),
                _ => throw new ArgumentException($"Tipo de reporte no soportado: {reportType}")
            };
        }
    }
}
