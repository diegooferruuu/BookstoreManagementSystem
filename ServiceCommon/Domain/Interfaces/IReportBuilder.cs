namespace ServiceCommon.Domain.Interfaces
{
    public interface IReportBuilder
    {
        IReportBuilder SetTitle(string title);
        IReportBuilder SetHeaders(List<string> headers);
        IReportBuilder AddRow(List<object> rowData);
        IReportBuilder AddRows(List<List<object>> rows);
        IReportBuilder SetMetadata(string author, string subject);
        IReportBuilder SetCreatedBy(string createdBy);
        IReportBuilder SetChartData(Dictionary<string, decimal> chartData);
        IReportBuilder SetProductChartData(Dictionary<string, decimal> productChartData);
        IReportService Build();
    }
}
