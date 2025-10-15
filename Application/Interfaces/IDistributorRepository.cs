using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;

namespace BookstoreManagementSystem.Application.Interfaces
{
    public interface IDistributorRepository
    {
        void Create(Distributor distributor);
        Distributor? Read(int id);
        void Update(Distributor distributor);
        void Delete(int id);
        List<Distributor> GetAll();
    }
}
