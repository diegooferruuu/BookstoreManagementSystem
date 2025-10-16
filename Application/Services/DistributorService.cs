using BookstoreManagementSystem.Application.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;

namespace BookstoreManagementSystem.Application.Services
{
    public class DistributorService
    {
        private readonly IDistributorRepository _repository;

        public DistributorService(IDistributorRepository repository)
        {
            _repository = repository;
        }

        public List<Distributor> GetAll() => _repository.GetAll();
        public Distributor? Read(int id) => _repository.Read(id);
        public void Create(Distributor distributor) => _repository.Create(distributor);
        public void Update(Distributor distributor) => _repository.Update(distributor);
        public void Delete(int id) => _repository.Delete(id);
    }
}
