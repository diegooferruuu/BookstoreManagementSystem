using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Application.Ports
{
    public interface IProductRepository : IDataBase<Product>
    {
        // Aquí se pueden agregar métodos específicos para productos si es necesario en el futuro
        // Por ejemplo: List<Product> GetByCategory(int categoryId);
        // Por ejemplo: List<Product> GetLowStock(int threshold);
    }
}
