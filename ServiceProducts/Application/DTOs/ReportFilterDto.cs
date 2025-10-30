namespace ServiceProducts.Application.DTOs;

public sealed class ReportFilterDto
{
    public decimal? PriceMin { get; init; }
    public decimal? PriceMax { get; init; }
    public Guid? CategoryId { get; init; }
}
