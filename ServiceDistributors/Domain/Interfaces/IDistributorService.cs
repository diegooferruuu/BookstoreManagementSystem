using ServiceDistributors.Domain.Models;

namespace ServiceDistributors.Domain.Interfaces
{
    public interface IDistributorService
    {
        List<Distributor> GetAll();
        Distributor? Read(Guid id);
        void Create(Distributor distributor);
        void Update(Distributor distributor);
        void Delete(Guid id);
    }
}
