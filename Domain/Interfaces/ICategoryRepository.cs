using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;

namespace BookstoreManagementSystem.Domain.Interfaces
{
    public interface ICategoryRepository : IDataBase<Category>
    {
        // Métodos adicionales específicos de Category si los necesitas
    }
}
