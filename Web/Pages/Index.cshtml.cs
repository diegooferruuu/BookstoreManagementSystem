using BookstoreManagementSystem.Application.Services;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreManagementSystem.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ClientService _clients;
    private readonly ProductService _products;
    private readonly DistributorService _distributors;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        _clients = new ClientService(new ClientRepository());
        _products = new ProductService(new ProductRepository());
        _distributors = new DistributorService(new DistributorRepository());
    }

    public int ClientsCount { get; private set; }
    public int ProductsCount { get; private set; }
    public int DistributorsCount { get; private set; }

    public void OnGet()
    {
        try
        {
            ClientsCount = _clients.GetAll().Count;
            ProductsCount = _products.GetAll().Count;
            DistributorsCount = _distributors.GetAll().Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading statistics on home page");
            ClientsCount = 0;
            ProductsCount = 0;
            DistributorsCount = 0;
        }
    }

}
