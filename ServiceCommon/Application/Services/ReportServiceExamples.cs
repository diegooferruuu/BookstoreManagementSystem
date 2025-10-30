using ServiceCommon.Application.Services;
using ServiceCommon.Domain.Interfaces;

namespace ServiceCommon.Application.Services
{
    /// <summary>
    /// Ejemplos de uso del servicio de generación de reportes
    /// </summary>
    public static class ReportServiceExamples
    {
        /// <summary>
        /// Ejemplo 1: Uso básico con el Builder directamente
        /// </summary>
        public static byte[] Example1_DirectBuilderUsage()
        {
            // Crear un builder de PDF
            IReportBuilder pdfBuilder = new PdfReportBuilder();

            // Construir el reporte paso a paso
            var reportService = pdfBuilder
                .SetTitle("Reporte de Ventas")
                .SetHeaders(new List<string> { "ID", "Producto", "Cantidad", "Precio" })
                .AddRow(new List<object> { 1, "Libro A", 10, 25.50 })
                .AddRow(new List<object> { 2, "Libro B", 5, 30.00 })
                .AddRow(new List<object> { 3, "Libro C", 8, 15.75 })
                .SetMetadata("Admin", "Ventas Mensuales")
                .Build();

            // Generar el reporte
            return reportService.GenerateReport();
        }

        /// <summary>
        /// Ejemplo 2: Uso con Factory
        /// </summary>
        public static byte[] Example2_FactoryUsage(ReportType reportType)
        {
            // Crear builder usando la factory
            var builder = ReportBuilderFactory.CreateBuilder(reportType);

            var data = new List<List<object>>
            {
                new() { 1, "Cliente A", "cliente.a@email.com", DateTime.Now },
                new() { 2, "Cliente B", "cliente.b@email.com", DateTime.Now },
                new() { 3, "Cliente C", "cliente.c@email.com", DateTime.Now }
            };

            var reportService = builder
                .SetTitle("Listado de Clientes")
                .SetHeaders(new List<string> { "ID", "Nombre", "Email", "Fecha Registro" })
                .AddRows(data)
                .SetMetadata("Sistema", "Clientes")
                .Build();

            return reportService.GenerateReport();
        }

        /// <summary>
        /// Ejemplo 3: Uso con Director
        /// </summary>
        public static byte[] Example3_DirectorUsage(ReportType reportType)
        {
            // Crear builder
            var builder = ReportBuilderFactory.CreateBuilder(reportType);
            
            // Crear director
            var director = new ReportDirector(builder);

            // Preparar datos
            var headers = new List<string> { "Producto", "Stock", "Precio", "Total" };
            var data = new List<List<object>>
            {
                new() { "Laptop", 15, 1200.00, 18000.00 },
                new() { "Mouse", 50, 25.50, 1275.00 },
                new() { "Teclado", 30, 45.00, 1350.00 }
            };

            // Construir reporte usando el director
            var reportService = director.ConstructReport(
                title: "Inventario de Productos",
                headers: headers,
                data: data,
                author: "Administrador",
                subject: "Inventario"
            );

            return reportService.GenerateReport();
        }

        /// <summary>
        /// Ejemplo 4: Generar ambos formatos (PDF y Excel) con los mismos datos
        /// </summary>
        public static (byte[] pdfReport, byte[] excelReport) Example4_BothFormats()
        {
            var headers = new List<string> { "ID", "Nombre", "Email", "Teléfono" };
            var data = new List<List<object>>
            {
                new() { 1, "Juan Pérez", "juan@email.com", "555-1234" },
                new() { 2, "María García", "maria@email.com", "555-5678" },
                new() { 3, "Carlos López", "carlos@email.com", "555-9012" }
            };

            // Generar PDF
            var pdfBuilder = ReportBuilderFactory.CreateBuilder(ReportType.Pdf);
            var pdfDirector = new ReportDirector(pdfBuilder);
            var pdfService = pdfDirector.ConstructReport("Directorio de Contactos", headers, data);
            var pdfBytes = pdfService.GenerateReport();

            // Generar Excel
            var excelBuilder = ReportBuilderFactory.CreateBuilder(ReportType.Excel);
            var excelDirector = new ReportDirector(excelBuilder);
            var excelService = excelDirector.ConstructReport("Directorio de Contactos", headers, data);
            var excelBytes = excelService.GenerateReport();

            return (pdfBytes, excelBytes);
        }

        /// <summary>
        /// Ejemplo 5: Uso en un controlador web (ejemplo conceptual)
        /// </summary>
        public static class WebControllerExample
        {
            // Simulación de uso en un endpoint
            public static (byte[] fileContent, string contentType, string fileName) GenerateReport(
                string format,
                List<string> headers,
                List<List<object>> data,
                string title)
            {
                // Determinar el tipo de reporte
                var reportType = format.ToLower() switch
                {
                    "pdf" => ReportType.Pdf,
                    "excel" or "xlsx" => ReportType.Excel,
                    _ => ReportType.Pdf
                };

                // Crear y generar reporte
                var builder = ReportBuilderFactory.CreateBuilder(reportType);
                var director = new ReportDirector(builder);
                var reportService = director.ConstructReport(title, headers, data);

                // Generar archivo
                var fileContent = reportService.GenerateReport();
                var contentType = reportService.GetContentType();
                var fileName = $"{title.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}{reportService.GetFileExtension()}";

                return (fileContent, contentType, fileName);
            }
        }
    }
}
