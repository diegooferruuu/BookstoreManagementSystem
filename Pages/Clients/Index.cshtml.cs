using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages.Clients
{
    public class IndexModel : PageModel
    {
        public List<Client> Clientes { get; private set; }

        public void OnGet()
        {
            var repo = new ClientRepository();
            Clientes = new List<Client>();

            for (int i = 1; i <= 10; i++) // Simulación: leer los primeros 10 clientes
            {
                var cliente = repo.Read(i);
                if (cliente != null)
                    Clientes.Add(cliente);
            }

        }
    }
}
