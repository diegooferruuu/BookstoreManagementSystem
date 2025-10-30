using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ServiceCommon.Domain.Interfaces;
using ServiceCommon.Domain.Models;

namespace ServiceCommon.Infrastructure.Reports
{
    public class PdfReportService : IReportService
    {
        private readonly ReportData _reportData;

        public PdfReportService(ReportData reportData)
        {
            _reportData = reportData;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateReport()
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Column(column =>
                        {
                            column.Spacing(10);
                            
                            // Logo y título
                            column.Item().Row(row =>
                            {
                                // Logo de libro abierto (usando emoji/texto)
                                row.ConstantItem(50).Height(50).Background(Colors.Blue.Medium)
                                    .AlignCenter()
                                    .AlignMiddle()
                                    .Text("📖")
                                    .FontSize(30);

                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text(_reportData.Title)
                                        .SemiBold()
                                        .FontSize(20)
                                        .FontColor(Colors.Blue.Medium);
                                    
                                    col.Item().Text($"Generado: {_reportData.GeneratedDate:dd/MM/yyyy HH:mm}")
                                        .FontSize(9)
                                        .FontColor(Colors.Grey.Darken2);
                                });
                            });

                            // Información del creador
                            if (!string.IsNullOrEmpty(_reportData.CreatedBy))
                            {
                                column.Item().Text($"Creado por: {_reportData.CreatedBy}")
                                    .FontSize(10)
                                    .FontColor(Colors.Grey.Darken1);
                            }
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(5);

                            // Metadatos
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"Autor: {_reportData.Author}");
                                row.RelativeItem().Text($"Asunto: {_reportData.Subject}").AlignRight();
                            });

                            column.Item().PaddingTop(10);

                            // Tabla
                            column.Item().Table(table =>
                            {
                                // Define las columnas
                                table.ColumnsDefinition(columns =>
                                {
                                    foreach (var _ in _reportData.Headers)
                                    {
                                        columns.RelativeColumn();
                                    }
                                });

                                // Headers
                                table.Header(header =>
                                {
                                    foreach (var headerText in _reportData.Headers)
                                    {
                                        header.Cell().Element(CellStyle).Text(headerText).SemiBold();
                                    }

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container
                                            .Border(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .Background(Colors.Grey.Lighten3)
                                            .Padding(5);
                                    }
                                });

                                // Rows
                                foreach (var row in _reportData.Rows)
                                {
                                    foreach (var cell in row)
                                    {
                                        table.Cell().Element(CellStyle).Text(cell?.ToString() ?? "");
                                    }

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container
                                            .Border(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5);
                                    }
                                }
                            });

                            // Gráfico de torta visual si hay datos
                            if (_reportData.ChartData != null && _reportData.ChartData.Any())
                            {
                                column.Item().PageBreak();
                                
                                column.Item().PaddingTop(20);
                                column.Item().Text("Distribución de Ventas por Cliente")
                                    .SemiBold()
                                    .FontSize(16)
                                    .FontColor(Colors.Blue.Medium);
                                
                                column.Item().PaddingTop(10);
                                
                                // Dibujar gráfico de barras horizontales como representación visual
                                var total = _reportData.ChartData.Values.Sum();
                                var colors = new[] 
                                { 
                                    Colors.Blue.Medium, 
                                    Colors.Orange.Medium, 
                                    Colors.Green.Medium,
                                    Colors.Red.Medium,
                                    Colors.Purple.Medium,
                                    Colors.Teal.Medium,
                                    Colors.Indigo.Medium,
                                    Colors.Pink.Medium
                                };
                                
                                var colorIndex = 0;
                                foreach (var item in _reportData.ChartData.OrderByDescending(x => x.Value).Take(8))
                                {
                                    var percentage = (double)item.Value / (double)total * 100;
                                    var barWidth = percentage;
                                    
                                    column.Item().PaddingTop(8).Column(col =>
                                    {
                                        col.Item().Text($"{item.Key}")
                                            .FontSize(10)
                                            .SemiBold();
                                        
                                        col.Item().PaddingTop(3).Row(row =>
                                        {
                                            row.RelativeItem((float)barWidth)
                                                .Height(25)
                                                .Background(colors[colorIndex % colors.Length])
                                                .AlignMiddle()
                                                .PaddingLeft(5)
                                                .Text($"${item.Value:N2} ({percentage:F1}%)")
                                                .FontSize(9)
                                                .FontColor(Colors.White);
                                            
                                            if (barWidth < 100)
                                                row.RelativeItem((float)(100 - barWidth));
                                        });
                                    });
                                    
                                    colorIndex++;
                                }
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                });
            });

            return document.GeneratePdf();
        }

        public string GetContentType()
        {
            return "application/pdf";
        }

        public string GetFileExtension()
        {
            return ".pdf";
        }
    }
}
