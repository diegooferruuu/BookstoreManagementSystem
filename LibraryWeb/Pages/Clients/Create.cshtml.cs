using ServiceClients.Domain.Models;
using ServiceClients.Domain.Interfaces;
using ServiceClients.Domain.Validations;
using ServiceCommon.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryWeb.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly IClientService _service;

        [BindProperty]
        public Client Client { get; set; } = new();

        public CreateModel(IClientService service)
        {
            _service = service;
        }

        public IActionResult OnPost()
        {
            foreach (var err in ClientValidation.Validate(Client))
                ModelState.AddModelError($"Client.{err.Field}", err.Message);

            if (!ModelState.IsValid)
                return Page();

            try
            {
                _service.Create(Client);
                return RedirectToPage("Index");
            }
            catch (ValidationException vex)
            {
                foreach (var e in vex.Errors)
                    ModelState.AddModelError($"Client.{e.Field}", e.Message);
                return Page();
            }
        }
    }
}
