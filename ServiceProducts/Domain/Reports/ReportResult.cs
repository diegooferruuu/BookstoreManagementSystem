namespace ServiceProducts.Domain.Reports;

public sealed class ReportResult
{
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = "reporte";
    public string MimeType { get; init; } = "application/octet-stream";
}
