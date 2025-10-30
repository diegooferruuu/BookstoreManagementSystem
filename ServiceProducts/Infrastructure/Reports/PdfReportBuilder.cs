using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ServiceProducts.Domain.Interfaces.Reports;
using ServiceProducts.Domain.Reports;

namespace ServiceProducts.Infrastructure.Reports;

public sealed class PdfReportBuilder : IReportBuilder
{
    private ProductReportData _data = new();
    private string _title = "";
    private string _generatedBy = "";
    private DateTimeOffset _generatedAt;
    private byte[]? _logo;
    private ReportResult _result = new();

    public void Reset()
    {
        _data = new();
        _title = "";
        _generatedBy = "";
        _generatedAt = DateTimeOffset.UtcNow;
        _logo = null;
        _result = new();
    }

    public void SetHeader(string title, string generatedBy, DateTimeOffset generatedAt, byte[]? logoBytes)
    {
        _title = title;
        _generatedBy = generatedBy;
        _generatedAt = generatedAt;
        _logo = logoBytes;
    }

    public void SetBody(ProductReportData data) => _data = data;

    public void SetFooter(string footerText) { /* se genera automáticamente */ }

    public ReportResult Build(string suggestedFileName)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        try
        {
            var bytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);

                    // Encabezado
                    page.Header().Element(Header);

                    // Contenido
                    page.Content().Element(Content);

                    // Pie de página
                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken2));
                        txt.Span("Página ").FontSize(9);
                        txt.CurrentPageNumber();
                        txt.Span("  |  ");
                        txt.Span($"Generado: {_generatedAt:dd/MM/yyyy HH:mm}");
                    });
                });
            }).GeneratePdf();

            _result = new ReportResult
            {
                Content = bytes,
                FileName = suggestedFileName.EndsWith(".pdf")
                    ? suggestedFileName
                    : $"{suggestedFileName}.pdf",
                MimeType = "application/pdf"
            };

            return _result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error al generar el reporte PDF: " + ex.Message, ex);
        }
    }

    // --------------------------------------------
    // ENCABEZADO: Logo + Nombre del sistema
    // --------------------------------------------
    private void Header(IContainer container)
    {
        container.Row(row =>
        {
            // Logo con ajuste seguro
            row.ConstantItem(60).AlignMiddle().AlignLeft().Element(c =>
            {
                if (_logo is { Length: > 0 })
                {
                    c.Element(x =>
                    {
                        x.Image(_logo)
                         .WithCompressionQuality(ImageCompressionQuality.Medium)
                         .FitArea();
                    });
                }
                else
                {
                    c.Text(""); // espacio si no hay logo
                }
            });

            // Nombre del sistema
            row.RelativeItem().AlignMiddle().Column(col =>
            {
                col.Item().Text("Librería")
                    .FontSize(18)
                    .SemiBold()
                    .FontColor(Colors.Blue.Medium);

                col.Item().Text("Sistema de Administración")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken2);

                col.Item().PaddingTop(4)
                    .Text($"Generado por: {_generatedBy}")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);
            });

            row.ConstantItem(30);
        });
    }

    // --------------------------------------------
    // CUERPO DEL REPORTE
    // --------------------------------------------
    private void Content(IContainer container)
    {
        container.PaddingTop(10).Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.ConstantColumn(40);   // Nro
                cols.RelativeColumn(2);    // Nombre
                cols.RelativeColumn(2);    // Categoría
                cols.RelativeColumn(3);    // Descripción
                cols.ConstantColumn(70);   // Precio
                cols.ConstantColumn(50);   // Stock
            });

            // Encabezado
            table.Header(header =>
            {
                header.Cell().Element(Th).Text("Nro");
                header.Cell().Element(Th).Text("Nombre");
                header.Cell().Element(Th).Text("Categoría");
                header.Cell().Element(Th).Text("Descripción");
                header.Cell().Element(Th).Text("Precio Bs.");
                header.Cell().Element(Th).Text("Stock");
            });

            // Filas
            foreach (var row in _data.Rows)
            {
                table.Cell().Element(Td).Text(row.Nro);
                table.Cell().Element(Td).Text(row.Name);
                table.Cell().Element(Td).Text(row.Category);
                table.Cell().Element(Td).Text(row.Description);
                table.Cell().Element(Td).AlignRight().Text(row.Price.ToString("0.00"));
                table.Cell().Element(Td).AlignRight().Text(row.Stock);
            }
        });

        // Estilos de celdas
        static IContainer Th(IContainer c) =>
            c.DefaultTextStyle(x => x.SemiBold().FontSize(11))
             .PaddingVertical(6)
             .PaddingHorizontal(4)
             .BorderBottom(1)
             .BorderColor(Colors.Grey.Medium)
             .Background(Colors.Grey.Lighten3)
             .AlignCenter();

        static IContainer Td(IContainer c) =>
            c.PaddingVertical(4)
             .PaddingHorizontal(4)
             .DefaultTextStyle(x => x.FontSize(10));
    }
}
