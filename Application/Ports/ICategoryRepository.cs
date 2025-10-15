using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Application.Ports
{
    public interface ICategoryRepository : IDataBase<Category>
    {
        // Aquí se pueden agregar métodos específicos para categorías si es necesario en el futuro
        // Por ejemplo: Category? GetByName(string name);
    }
}
