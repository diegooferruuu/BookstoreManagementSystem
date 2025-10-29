using ServiceClients.Domain.Models;

namespace ServiceClients.Domain.Interfaces
{
    public interface IClientRepository
    {
        List<Client> GetAll();
        Client? Read(Guid id);
        void Create(Client client);
        void Update(Client client);
        void Delete(Guid id);
    }
}
