using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Services;

namespace BookstoreManagementSystem.Application.Interfaces
{
    public interface IClientRepository : IDataBase<Client>
    {
        // Métodos adicionales específicos de Client si los necesitas
    }
}
