using ClosedXML.Excel;
using ServiceProducts.Domain.Interfaces.Reports;
using ServiceProducts.Domain.Reports;
using System.Drawing;

namespace ServiceProducts.Infrastructure.Reports;

public sealed class ExcelReportBuilder : IReportBuilder
{
    private ProductReportData _data = new();
    private string _title = "";
    private string _generatedBy = "";
    private DateTimeOffset _generatedAt;

    public void Reset()
    {
        _data = new();
        _title = "";
        _generatedBy = "";
        _generatedAt = DateTimeOffset.UtcNow;
    }

    public void SetHeader(string title, string generatedBy, DateTimeOffset generatedAt, byte[]? logoBytes)
    {
        _title = title;
        _generatedBy = generatedBy;
        _generatedAt = generatedAt;
    }

    public void SetBody(ProductReportData data) => _data = data;

    public void SetFooter(string footerText) { }

    public ReportResult Build(string suggestedFileName)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Productos");

        ws.Cell(1, 1).Value = _title;
        ws.Range(1, 1, 1, 6).Merge().Style.Font.SetBold().Font.SetFontSize(14);
        ws.Cell(2, 1).Value = $"Generado por: {_generatedBy} — {_generatedAt:dd/MM/yyyy HH:mm}";
        ws.Range(2, 1, 2, 6).Merge().Style.Font.SetFontSize(10).Font.SetItalic();

        ws.Cell(4, 1).Value = "Nro";
        ws.Cell(4, 2).Value = "Nombre";
        ws.Cell(4, 3).Value = "Categoría";
        ws.Cell(4, 4).Value = "Descripción";
        ws.Cell(4, 5).Value = "Precio Bs.";
        ws.Cell(4, 6).Value = "Stock";
        ws.Range(4, 1, 4, 6).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.FromHtml("#E8EEF7"));

        int r = 5;
        foreach (var row in _data.Rows)
        {
            ws.Cell(r, 1).Value = row.Nro;
            ws.Cell(r, 2).Value = row.Name;
            ws.Cell(r, 3).Value = row.Category;
            ws.Cell(r, 4).Value = row.Description;
            ws.Cell(r, 5).Value = row.Price;
            ws.Cell(r, 6).Value = row.Stock;
            r++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);

        return new ReportResult
        {
            Content = ms.ToArray(),
            FileName = suggestedFileName.EndsWith(".xlsx") ? suggestedFileName : $"{suggestedFileName}.xlsx",
            MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };
    }
}
