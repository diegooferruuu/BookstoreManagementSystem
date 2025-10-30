namespace ServiceProducts.Domain.Reports;

public sealed class ProductReportRow
{
    public int Nro { get; init; }
    public string Name { get; init; } = "";
    public string Category { get; init; } = "";
    public string Description { get; init; } = "";
    public decimal Price { get; init; }
    public int Stock { get; init; }
}

public sealed class ProductReportData
{
    public string Title { get; init; } = "LISTA DE PRODUCTOS";
    public string GeneratedBy { get; init; } = "";
    public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;
    public IReadOnlyList<ProductReportRow> Rows { get; init; } = Array.Empty<ProductReportRow>();
}
