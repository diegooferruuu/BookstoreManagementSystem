using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceSales.Domain.Interfaces;
using ServiceSales.Domain.Models;
using ServiceClients.Domain.Interfaces;
using ServiceUsers.Domain.Interfaces;

namespace LibraryWeb.Pages.Sales.Reports
{
    public class IndexModel : PageModel
    {
        private readonly ISalesReportService _salesReportService;
        private readonly IClientService _clientService;
        private readonly IUserService _userService;

        public IndexModel(
            ISalesReportService salesReportService,
            IClientService clientService,
            IUserService userService)
        {
            _salesReportService = salesReportService;
            _clientService = clientService;
            _userService = userService;
        }

        [BindProperty]
        public SaleReportFilter Filter { get; set; } = new();

        public List<SelectListItem> Users { get; set; } = new();
        public List<SelectListItem> Clients { get; set; } = new();

        public void OnGet()
        {
            LoadDropdowns();
        }

        public async Task<IActionResult> OnPostGeneratePdfAsync()
        {
            return await GenerateReportAsync("pdf");
        }

        public async Task<IActionResult> OnPostGenerateExcelAsync()
        {
            return await GenerateReportAsync("excel");
        }

        private async Task<IActionResult> GenerateReportAsync(string reportType)
        {
            try
            {
                // Obtener el username del usuario actual
                var currentUsername = User.Identity?.Name ?? "Sistema";
                
                var reportBytes = await _salesReportService.GenerateSalesReportAsync(Filter, reportType, currentUsername);
                var contentType = await _salesReportService.GetReportContentType(reportType);
                var extension = await _salesReportService.GetReportFileExtension(reportType);
                
                var fileName = $"Reporte_Ventas_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";

                return File(reportBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al generar el reporte: {ex.Message}";
                LoadDropdowns();
                return Page();
            }
        }

        private void LoadDropdowns()
        {
            // Cargar usuarios
            var users = _userService.GetAll();
            Users = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Todos los usuarios --" }
            };
            Users.AddRange(users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = $"{u.Username}".Trim()
            }));

            // Cargar clientes
            var clients = _clientService.GetAll();
            Clients = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Todos los clientes --" }
            };
            Clients.AddRange(clients.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.FirstName} {c.LastName}".Trim()
            }));
        }
    }
}
