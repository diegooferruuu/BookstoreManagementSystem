using ServiceCommon.Domain.Interfaces;
using ServiceCommon.Domain.Models;
using ServiceCommon.Infrastructure.Reports;

namespace ServiceCommon.Application.Services
{
    public class PdfReportBuilder : IReportBuilder
    {
        private readonly ReportData _reportData;

        public PdfReportBuilder()
        {
            _reportData = new ReportData();
        }

        public IReportBuilder SetTitle(string title)
        {
            _reportData.Title = title;
            return this;
        }

        public IReportBuilder SetHeaders(List<string> headers)
        {
            _reportData.Headers = headers;
            return this;
        }

        public IReportBuilder AddRow(List<object> rowData)
        {
            _reportData.Rows.Add(rowData);
            return this;
        }

        public IReportBuilder AddRows(List<List<object>> rows)
        {
            _reportData.Rows.AddRange(rows);
            return this;
        }

        public IReportBuilder SetMetadata(string author, string subject)
        {
            _reportData.Author = author;
            _reportData.Subject = subject;
            return this;
        }

        public IReportBuilder SetCreatedBy(string createdBy)
        {
            _reportData.CreatedBy = createdBy;
            return this;
        }

        public IReportBuilder SetChartData(Dictionary<string, decimal> chartData)
        {
            _reportData.ChartData = chartData;
            return this;
        }

        public IReportService Build()
        {
            return new PdfReportService(_reportData);
        }
    }
}
