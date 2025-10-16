using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Domain.Interfaces
{
    public interface IDistributorService
    {
        public List<Distributor> GetAll() ;
        public Distributor? Read(Guid id);
        public void Create(Distributor distributor);
        public void Update(Distributor distributor) ;
        public void Delete(Guid id);

    }
}
