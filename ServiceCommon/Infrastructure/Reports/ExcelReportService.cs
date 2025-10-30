using ClosedXML.Excel;
using ServiceCommon.Domain.Interfaces;
using ServiceCommon.Domain.Models;

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
    }
}
