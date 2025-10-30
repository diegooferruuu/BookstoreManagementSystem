namespace ServiceCommon.Domain.Models
{
    public class ReportData
    {
        public string Title { get; set; } = string.Empty;
        public List<string> Headers { get; set; } = new();
        public List<List<object>> Rows { get; set; } = new();
        public string Author { get; set; } = "Sistema";
        public string Subject { get; set; } = "Reporte";
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public Dictionary<string, decimal>? ChartData { get; set; }
        public Dictionary<string, decimal>? ProductChartData { get; set; }
        public Dictionary<string, decimal>? ProductRevenueData { get; set; }
    }
}
