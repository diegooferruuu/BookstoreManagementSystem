using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Application.Ports
{
    public interface IClientRepository : IDataBase<Client>
    {
        // Aquí se pueden agregar métodos específicos para clientes si es necesario en el futuro
        // Por ejemplo: List<Client> SearchByName(string name);
    }
}
