using ClosedXML.Excel;
using ServiceCommon.Domain.Interfaces;
using ServiceCommon.Domain.Models;
using SkiaSharp;
using System.Linq;

namespace ServiceCommon.Infrastructure.Reports
{
    public class ExcelReportService : IReportService
    {
        private readonly ReportData _reportData;

        public ExcelReportService(ReportData reportData)
        {
            _reportData = reportData;
        }

        public byte[] GenerateReport()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte");

            // Título
            worksheet.Cell(1, 1).Value = _reportData.Title;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, _reportData.Headers.Count).Merge();

            // Metadatos
            worksheet.Cell(2, 1).Value = $"Autor: {_reportData.Author}";
            worksheet.Cell(3, 1).Value = $"Fecha: {_reportData.GeneratedDate:dd/MM/yyyy HH:mm}";
            worksheet.Cell(3, _reportData.Headers.Count).Value = _reportData.Subject;

            // Headers (fila 5)
            int headerRow = 5;
            for (int i = 0; i < _reportData.Headers.Count; i++)
            {
                var cell = worksheet.Cell(headerRow, i + 1);
                cell.Value = _reportData.Headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data rows
            int currentRow = headerRow + 1;
            foreach (var row in _reportData.Rows)
            {
                for (int i = 0; i < row.Count && i < _reportData.Headers.Count; i++)
                {
                    var cell = worksheet.Cell(currentRow, i + 1);
                    var value = row[i];

                    if (value is DateTime dateValue)
                    {
                        cell.Value = dateValue;
                        cell.Style.DateFormat.Format = "dd/MM/yyyy";
                    }
                    else if (value is decimal || value is double || value is float || value is int || value is long)
                    {
                        cell.Value = Convert.ToDouble(value);
                        if (value is decimal || value is double || value is float)
                        {
                            cell.Style.NumberFormat.Format = "#,##0.00";
                        }
                    }
                    else
                    {
                        cell.Value = value?.ToString() ?? "";
                    }

                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
                currentRow++;
            }

            // Ajustar ancho de columnas
            worksheet.Columns().AdjustToContents();

            // --- Hoja con gráfico de tortas (7+Otros) ---
            if (_reportData.ChartData != null && _reportData.ChartData.Any())
            {
                var chartSheet = workbook.Worksheets.Add("Gráfico");

                chartSheet.Cell(1, 1).Value = "Gráfico de Tortas - Distribución de Ventas";
                chartSheet.Cell(1, 1).Style.Font.Bold = true;
                chartSheet.Cell(1, 1).Style.Font.FontSize = 16;

                // Preparar 7+Otros
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

                var pieBytes = CreatePieChartImage(items, skiaColors);
                using (var imgStream = new MemoryStream(pieBytes))
                {
                    var picture = chartSheet.AddPicture(imgStream)
                        .MoveTo(chartSheet.Cell(3, 1))
                        .Scale(0.55);
                    picture.Name = "PieChart";
                }

                // Leyenda a la derecha
                chartSheet.Column(11).Width = 4;  // K: cuadrito color
                chartSheet.Column(12).Width = 45; // L: texto

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var percentage = total == 0 ? 0 : (double)item.Value / (double)total * 100d;
                    var color = skiaColors[i % skiaColors.Length];

                    var boxCell = chartSheet.Cell(3 + i, 11); // K
                    boxCell.Value = "";
                    boxCell.Style.Fill.BackgroundColor = XLColor.FromArgb(color.Red, color.Green, color.Blue);
                    boxCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    chartSheet.Row(3 + i).Height = 18;

                    var textCell = chartSheet.Cell(3 + i, 12); // L
                    textCell.Value = $"{item.Key}: ${item.Value:N2} ({percentage:F1}%)";
                }

                chartSheet.Cell(2, 1).Value = "Incluye 7 categorías principales y 'Otros' cuando aplica.";
                chartSheet.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;
                chartSheet.Cell(2, 1).Style.Font.FontSize = 10;
            }

            // --- Hoja con gráfico de productos más vendidos ---
            if (_reportData.ProductChartData != null && _reportData.ProductChartData.Any())
            {
                var productChartSheet = workbook.Worksheets.Add("Gráfico Productos");

                productChartSheet.Cell(1, 1).Value = "Gráfico de Tortas - Productos Más Vendidos";
                productChartSheet.Cell(1, 1).Style.Font.Bold = true;
                productChartSheet.Cell(1, 1).Style.Font.FontSize = 16;

                // Preparar 7+Otros
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

                var pieProductsBytes = CreatePieChartImage(productItems, skiaColorsProducts);
                using (var imgStream = new MemoryStream(pieProductsBytes))
                {
                    var picture = productChartSheet.AddPicture(imgStream)
                        .MoveTo(productChartSheet.Cell(3, 1))
                        .Scale(0.55);
                    picture.Name = "PieChartProducts";
                }

                // Leyenda a la derecha
                productChartSheet.Column(11).Width = 4;  // K: cuadrito color
                productChartSheet.Column(12).Width = 50; // L: texto

                for (int i = 0; i < productItems.Count; i++)
                {
                    var item = productItems[i];
                    var percentage = totalProducts == 0 ? 0 : (double)item.Value / (double)totalProducts * 100d;
                    var color = skiaColorsProducts[i % skiaColorsProducts.Length];

                    var boxCell = productChartSheet.Cell(3 + i, 11); // K
                    boxCell.Value = "";
                    boxCell.Style.Fill.BackgroundColor = XLColor.FromArgb(color.Red, color.Green, color.Blue);
                    boxCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    productChartSheet.Row(3 + i).Height = 18;

                    var textCell = productChartSheet.Cell(3 + i, 12); // L
                    textCell.Value = $"{item.Key}: {item.Value:N0} unidades ({percentage:F1}%)";
                }

                productChartSheet.Cell(2, 1).Value = "Muestra la cantidad total de unidades vendidas por producto. Incluye 7 productos principales y 'Otros' cuando aplica.";
                productChartSheet.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;
                productChartSheet.Cell(2, 1).Style.Font.FontSize = 10;
            }

            // Convertir a bytes
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public string GetContentType()
        {
            return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        public string GetFileExtension()
        {
            return ".xlsx";
        }

        // Genera una imagen PNG en memoria con el gráfico de tortas
        private static byte[] CreatePieChartImage(List<KeyValuePair<string, decimal>> items, SKColor[] palette)
        {
            const int size = 800;
            int width = size;
            int height = size;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            var cx = width / 2f;
            var cy = height / 2f;
            var radius = Math.Min(width, height) * 0.42f;
            var rect = new SKRect(cx - radius, cy - radius, cx + radius, cy + radius);

            using (var shadowPaint = new SKPaint { IsAntialias = true, Color = new SKColor(0, 0, 0, 30), ImageFilter = SKImageFilter.CreateBlur(10, 10) })
            {
                var shadowRect = rect; shadowRect.Offset(6, 6);
                canvas.DrawOval(shadowRect, shadowPaint);
            }

            var total = items.Sum(i => (double)i.Value);
            float startAngle = -90f;

            for (int i = 0; i < items.Count; i++)
            {
                var sweep = total <= 0 ? 0f : (float)((double)items[i].Value / total * 360f);
                var color = palette[i % palette.Length];

                using var slicePaint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = color };
                canvas.DrawArc(rect, startAngle, sweep, true, slicePaint);

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
