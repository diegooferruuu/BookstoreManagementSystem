using ServiceCommon.Application.Services;
using ServiceCommon.Domain.Interfaces;
using ServiceSales.Domain.Interfaces;
using ServiceSales.Domain.Models;

namespace ServiceSales.Application.Services
{
    public class SalesReportService : ISalesReportService
    {
        private readonly ISaleRepository _saleRepository;

        public SalesReportService(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<byte[]> GenerateSalesReportAsync(SaleReportFilter filter, string reportType, string createdBy = "Sistema", CancellationToken ct = default)
        {
            // Obtener datos filtrados
            var sales = await _saleRepository.GetFilteredSalesAsync(filter, ct);

            // Preparar datos para el reporte
            var title = "Reporte de Ventas";
            var headers = new List<string>
            {
                "Fecha Venta",
                "Cliente",
                "Vendedor",
                "Método Pago",
                "Total",
                "Estado",
                "Notas"
            };

            var rows = sales.Select(sale => new List<object>
            {
                sale.SaleDate,
                sale.ClientName ?? "N/A",
                sale.UserName ?? "N/A",
                sale.PaymentMethod,
                sale.Total,
                sale.Status,
                sale.Notes ?? ""
            }).ToList();

            // Calcular datos para el gráfico de torta (ventas por cliente)
            var chartData = sales
                .GroupBy(s => s.ClientName ?? "Sin Cliente")
                .Select(g => new { Client = g.Key, Total = g.Sum(s => s.Total) })
                .OrderByDescending(x => x.Total)
                .ToDictionary(x => x.Client, x => x.Total);

            // Agregar fila de totales
            if (sales.Any())
            {
                rows.Add(new List<object>
                {
                    "",
                    "",
                    "TOTAL:",
                    "",
                    sales.Sum(s => s.Total),
                    $"{sales.Count} ventas",
                    ""
                });
            }

            // Determinar tipo de reporte
            var reportTypeEnum = reportType.ToLower() switch
            {
                "excel" => ReportType.Excel,
                "pdf" => ReportType.Pdf,
                _ => ReportType.Pdf
            };

            // Crear builder según el tipo
            var builder = ReportBuilderFactory.CreateBuilder(reportTypeEnum);

            // Construir metadatos
            var filterDescription = BuildFilterDescription(filter);

            // Usar el director para construir el reporte
            var director = new ReportDirector(builder);
            
            // Configurar el builder con todos los datos
            builder
                .SetTitle(title)
                .SetHeaders(headers)
                .AddRows(rows)
                .SetMetadata("Sistema de Ventas", $"Reporte de Ventas - {filterDescription}")
                .SetCreatedBy(createdBy);

            // Solo agregar gráfico en PDF si hay datos
            if (reportType.ToLower() == "pdf" && chartData.Any())
            {
                builder.SetChartData(chartData);
            }

            var reportService = builder.Build();

            // Generar y retornar el reporte
            return reportService.GenerateReport();
        }

        public Task<string> GetReportContentType(string reportType)
        {
            var contentType = reportType.ToLower() switch
            {
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "pdf" => "application/pdf",
                _ => "application/pdf"
            };

            return Task.FromResult(contentType);
        }

        public Task<string> GetReportFileExtension(string reportType)
        {
            var extension = reportType.ToLower() switch
            {
                "excel" => ".xlsx",
                "pdf" => ".pdf",
                _ => ".pdf"
            };

            return Task.FromResult(extension);
        }

        private string BuildFilterDescription(SaleReportFilter filter)
        {
            var parts = new List<string>();

            if (filter.UserId.HasValue)
                parts.Add($"Usuario: {filter.UserId}");

            if (filter.ClientId.HasValue)
                parts.Add($"Cliente: {filter.ClientId}");

            if (filter.StartDate.HasValue)
                parts.Add($"Desde: {filter.StartDate:dd/MM/yyyy}");

            if (filter.EndDate.HasValue)
                parts.Add($"Hasta: {filter.EndDate:dd/MM/yyyy}");

            return parts.Any() ? string.Join(", ", parts) : "Todos los registros";
        }
    }
}
