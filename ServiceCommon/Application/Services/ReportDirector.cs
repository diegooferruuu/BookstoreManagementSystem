using ServiceCommon.Domain.Interfaces;

namespace ServiceCommon.Application.Services
{
    public class ReportDirector
    {
        private readonly IReportBuilder _builder;

        public ReportDirector(IReportBuilder builder)
        {
            _builder = builder;
        }

        public IReportService ConstructReport(
            string title,
            List<string> headers,
            List<List<object>> data,
            string author = "Sistema",
            string subject = "Reporte")
        {
            return _builder
                .SetTitle(title)
                .SetHeaders(headers)
                .AddRows(data)
                .SetMetadata(author, subject)
                .Build();
        }

        public IReportService ConstructEmptyReport(string title)
        {
            return _builder
                .SetTitle(title)
                .SetHeaders(new List<string>())
                .SetMetadata("Sistema", "Reporte")
                .Build();
        }
    }
}
