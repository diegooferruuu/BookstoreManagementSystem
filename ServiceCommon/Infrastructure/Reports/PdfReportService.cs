using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ServiceCommon.Domain.Interfaces;
using ServiceCommon.Domain.Models;
using SkiaSharp;

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

                            // Gráfico de tortas si hay datos
                            if (_reportData.ChartData != null && _reportData.ChartData.Any())
                            {
                                column.Item().PageBreak();
                                
                                column.Item().PaddingTop(20);
                                column.Item().Text("Gráfico de Tortas - Distribución de Ventas")
                                    .SemiBold()
                                    .FontSize(16)
                                    .FontColor(Colors.Blue.Medium);
                                
                                column.Item().PaddingTop(10);

                                // Preparar datos y paleta de colores (7 principales + Otros)
                                var ordered = _reportData.ChartData
                                    .OrderByDescending(x => x.Value)
                                    .ToList();

                                List<KeyValuePair<string, decimal>> items;
                                if (ordered.Count > 8)
                                {
                                    var top7 = ordered.Take(7).ToList();
                                    var othersTotal = ordered.Skip(7).Sum(x => x.Value);
                                    top7.Add(new KeyValuePair<string, decimal>("Otros", othersTotal));
                                    items = top7;
                                }
                                else
                                {
                                    items = ordered;
                                }

                                var total = items.Sum(x => x.Value);

                                // Paleta para Skia (imagen)
                                var skiaColors = new SKColor[]
                                {
                                    SKColors.SteelBlue,
                                    SKColors.Orange,
                                    SKColors.MediumSeaGreen,
                                    SKColors.IndianRed,
                                    SKColors.MediumPurple,
                                    SKColors.Teal,
                                    SKColors.Indigo,
                                    SKColors.HotPink
                                };

                                // Paleta para legendas en QuestPDF (mismo orden que Skia)
                                var questColors = new[]
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

                                // Generar imagen del gráfico de tortas con SkiaSharp
                                var pieImage = CreatePieChartImage(items, skiaColors);

                                // Layout: gráfico + leyenda
                                column.Item().Row(r =>
                                {
                                    r.ConstantItem(320).Height(320).Image(pieImage).FitArea();

                                    r.RelativeItem().PaddingLeft(20).Column(legend =>
                                    {
                                        legend.Spacing(6);

                                        for (int i = 0; i < items.Count; i++)
                                        {
                                            var item = items[i];
                                            var percentage = total == 0 ? 0 : (double)item.Value / (double)total * 100d;
                                            var color = questColors[i % questColors.Length];

                                            legend.Item().Row(row =>
                                            {
                                                row.ConstantItem(12).Height(12).Background(color).Border(1).BorderColor(Colors.Grey.Lighten1);
                                                row.RelativeItem().PaddingLeft(6).Text($"{item.Key}: ${item.Value:N2} ({percentage:F1}%)");
                                            });
                                        }
                                    });
                                });

                                // Nota
                                column.Item().PaddingTop(6).Text("Incluye 7 categorías principales y 'Otros' cuando aplica.")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            }

                            // Tabla de ingresos por producto
                            if (_reportData.ProductRevenueData != null && _reportData.ProductRevenueData.Any())
                            {
                                column.Item().PageBreak();
                                
                                column.Item().PaddingTop(20);
                                column.Item().Text("Ingresos por Producto")
                                    .SemiBold()
                                    .FontSize(16)
                                    .FontColor(Colors.Blue.Medium);
                                
                                column.Item().PaddingTop(10);

                                // Ordenar por ingresos descendente
                                var orderedRevenue = _reportData.ProductRevenueData
                                    .OrderByDescending(x => x.Value)
                                    .ToList();

                                var totalRevenue = orderedRevenue.Sum(x => x.Value);

                                // Tabla
                                column.Item().Table(table =>
                                {
                                    // Definir columnas: Producto, Ingresos, Porcentaje
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3); // Producto
                                        columns.RelativeColumn(2); // Ingresos
                                        columns.RelativeColumn(1); // Porcentaje
                                    });

                                    // Headers
                                    table.Header(header =>
                                    {
                                        header.Cell().Element(HeaderStyle).Text("Producto").SemiBold();
                                        header.Cell().Element(HeaderStyle).Text("Ingresos").SemiBold();
                                        header.Cell().Element(HeaderStyle).Text("% del Total").SemiBold();

                                        static IContainer HeaderStyle(IContainer container)
                                        {
                                            return container
                                                .Border(1)
                                                .BorderColor(Colors.Grey.Lighten2)
                                                .Background(Colors.Blue.Lighten4)
                                                .Padding(8);
                                        }
                                    });

                                    // Rows
                                    foreach (var item in orderedRevenue)
                                    {
                                        var percentage = totalRevenue == 0 ? 0 : (double)item.Value / (double)totalRevenue * 100d;

                                        table.Cell().Element(CellStyle).Text(item.Key);
                                        table.Cell().Element(CellStyle).Text($"${item.Value:N2}");
                                        table.Cell().Element(CellStyle).Text($"{percentage:F1}%");

                                        static IContainer CellStyle(IContainer container)
                                        {
                                            return container
                                                .Border(1)
                                                .BorderColor(Colors.Grey.Lighten2)
                                                .Padding(8);
                                        }
                                    }

                                    // Fila de totales
                                    table.Cell().Element(TotalStyle).Text("TOTAL").SemiBold();
                                    table.Cell().Element(TotalStyle).Text($"${totalRevenue:N2}").SemiBold();
                                    table.Cell().Element(TotalStyle).Text("100%").SemiBold();

                                    static IContainer TotalStyle(IContainer container)
                                    {
                                        return container
                                            .Border(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .Background(Colors.Grey.Lighten3)
                                            .Padding(8);
                                    }
                                });

                                // Nota
                                column.Item().PaddingTop(10).Text($"Total de productos diferentes: {orderedRevenue.Count}")
                                    .FontSize(10)
                                    .FontColor(Colors.Grey.Darken1);
                            }

                            // Gráfico de productos más vendidos
                            if (_reportData.ProductChartData != null && _reportData.ProductChartData.Any())
                            {
                                column.Item().PageBreak();
                                
                                column.Item().PaddingTop(20);
                                column.Item().Text("Gráfico de Tortas - Productos Más Vendidos")
                                    .SemiBold()
                                    .FontSize(16)
                                    .FontColor(Colors.Blue.Medium);
                                
                                column.Item().PaddingTop(10);

                                // Preparar datos y paleta de colores (7 principales + Otros)
                                var orderedProducts = _reportData.ProductChartData
                                    .OrderByDescending(x => x.Value)
                                    .ToList();

                                List<KeyValuePair<string, decimal>> productItems;
                                if (orderedProducts.Count > 8)
                                {
                                    var top7 = orderedProducts.Take(7).ToList();
                                    var othersTotal = orderedProducts.Skip(7).Sum(x => x.Value);
                                    top7.Add(new KeyValuePair<string, decimal>("Otros", othersTotal));
                                    productItems = top7;
                                }
                                else
                                {
                                    productItems = orderedProducts;
                                }

                                var totalProducts = productItems.Sum(x => x.Value);

                                // Paleta para Skia (imagen)
                                var skiaColorsProducts = new SKColor[]
                                {
                                    SKColors.DodgerBlue,
                                    SKColors.Coral,
                                    SKColors.LimeGreen,
                                    SKColors.Tomato,
                                    SKColors.Orchid,
                                    SKColors.DeepSkyBlue,
                                    SKColors.Gold,
                                    SKColors.Crimson
                                };

                                // Paleta para legendas en QuestPDF (mismo orden que Skia)
                                var questColorsProducts = new[]
                                {
                                    Colors.Blue.Darken1,
                                    Colors.Orange.Darken1,
                                    Colors.Green.Darken1,
                                    Colors.Red.Darken1,
                                    Colors.Purple.Darken1,
                                    Colors.Cyan.Darken1,
                                    Colors.Yellow.Darken2,
                                    Colors.Pink.Darken2
                                };

                                // Generar imagen del gráfico de tortas con SkiaSharp
                                var pieImageProducts = CreatePieChartImage(productItems, skiaColorsProducts);

                                // Layout: gráfico + leyenda
                                column.Item().Row(r =>
                                {
                                    r.ConstantItem(320).Height(320).Image(pieImageProducts).FitArea();

                                    r.RelativeItem().PaddingLeft(20).Column(legend =>
                                    {
                                        legend.Spacing(6);

                                        for (int i = 0; i < productItems.Count; i++)
                                        {
                                            var item = productItems[i];
                                            var percentage = totalProducts == 0 ? 0 : (double)item.Value / (double)totalProducts * 100d;
                                            var color = questColorsProducts[i % questColorsProducts.Length];

                                            legend.Item().Row(row =>
                                            {
                                                row.ConstantItem(12).Height(12).Background(color).Border(1).BorderColor(Colors.Grey.Lighten1);
                                                row.RelativeItem().PaddingLeft(6).Text($"{item.Key}: {item.Value:N0} unidades ({percentage:F1}%)");
                                            });
                                        }
                                    });
                                });

                                // Nota
                                column.Item().PaddingTop(6).Text("Muestra la cantidad total de unidades vendidas por producto. Incluye 7 productos principales y 'Otros' cuando aplica.")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
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

        // Genera una imagen PNG en memoria con el gráfico de tortas
        private static byte[] CreatePieChartImage(List<KeyValuePair<string, decimal>> items, SKColor[] palette)
        {
            const int size = 600; // imagen cuadrada
            int width = size;
            int height = size;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            // Área del círculo
            var cx = width / 2f;
            var cy = height / 2f;
            var radius = Math.Min(width, height) * 0.42f;
            var rect = new SKRect(cx - radius, cy - radius, cx + radius, cy + radius);

            // Sombra suave
            using (var shadowPaint = new SKPaint { IsAntialias = true, Color = new SKColor(0, 0, 0, 30), ImageFilter = SKImageFilter.CreateBlur(10, 10) })
            {
                var shadowRect = rect; shadowRect.Offset(4, 4);
                canvas.DrawOval(shadowRect, shadowPaint);
            }

            var total = items.Sum(i => (double)i.Value);
            float startAngle = -90f; // comenzar arriba

            for (int i = 0; i < items.Count; i++)
            {
                var sweep = total <= 0 ? 0f : (float)((double)items[i].Value / total * 360f);
                var color = palette[i % palette.Length];

                using var slicePaint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = color };
                canvas.DrawArc(rect, startAngle, sweep, true, slicePaint);

                // borde separador
                using var borderPaint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 2, Color = SKColors.White };
                canvas.DrawArc(rect, startAngle, sweep, true, borderPaint);

                startAngle += sweep;
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }
    }
}
