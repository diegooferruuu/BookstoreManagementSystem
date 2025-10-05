using BookstoreManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public string ConnectionMessage { get; private set; }

    public void OnGet()
    {

        try
        {
            var connection = DataBaseConnection.Instance.GetConnection();
            ConnectionMessage = "✅ Conexión exitosa a la base de datos.";
            connection.Close();
        }
        catch (Exception ex)
        {
            ConnectionMessage = "❌ Error al conectar: " + ex.Message;
        }

    }
}
