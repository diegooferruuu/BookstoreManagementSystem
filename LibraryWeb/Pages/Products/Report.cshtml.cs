using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceProducts.Application.DTOs;
using ServiceProducts.Domain.Interfaces;
using ServiceProducts.Domain.Interfaces.Reports;

namespace LibraryWeb.Pages.Products
{
    [Authorize(Policy = "RequireEmployeeOrAdmin")]
    public class ReportModel : PageModel
    {
        private readonly IProductReportService _reportService;
        private readonly IWebHostEnvironment _env;

        public ReportModel(IProductReportService reportService, IWebHostEnvironment env)
        {
            _reportService = reportService;
            _env = env;
        }

        [BindProperty]
        public decimal? PriceMin { get; set; }

        [BindProperty]
        public decimal? PriceMax { get; set; }

        [BindProperty]
        public string Format { get; set; } = "pdf";

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync([FromServices] IProductRepository repo)
        {
            var all = repo.GetAll();
            if (all.Any())
            {
                PriceMin = 0;
                PriceMax = all.Max(p => p.Price);
            }
        }

        public async Task<IActionResult> OnPostDownloadAsync(CancellationToken ct)
        {
            // Validaciones básicas
            if (PriceMin.HasValue && PriceMax.HasValue && PriceMin > PriceMax)
            {
                ErrorMessage = "El precio mínimo no puede ser mayor que el precio máximo.";
                return Page();
            }

            // Preparar filtros
            var filter = new ReportFilterDto
            {
                PriceMin = PriceMin,
                PriceMax = PriceMax
            };

            // Cargar logo opcional
            byte[]? logo = null;
            try
            {
                var logoPath = Path.Combine(_env.WebRootPath ?? "", "img", "logo.png");
                if (System.IO.File.Exists(logoPath))
                    logo = await System.IO.File.ReadAllBytesAsync(logoPath, ct);
            }
            catch
            {
                // No interrumpe si el logo no existe
            }

            // Generar reporte
            var generatedBy = User.Identity?.Name ?? "usuario";
            var result = await _reportService.GenerateAsync(filter, Format, generatedBy, logo, ct);

            if (result == null || result.Content.Length == 0)
            {
                ErrorMessage = "No se pudo generar el reporte.";
                return Page();
            }

            return File(result.Content, result.MimeType, result.FileName);
        }
    }
}
