using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Application.Ports
{
    public interface IDistributorRepository : IDataBase<Distributor>
    {
        // Aquí se pueden agregar métodos específicos para distribuidores si es necesario en el futuro
        // Por ejemplo: Distributor? GetByEmail(string email);
    }
}
