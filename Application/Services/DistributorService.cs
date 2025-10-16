using BookstoreManagementSystem.Application.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Validations;
using System.Collections.Generic;
using System.Linq;

namespace BookstoreManagementSystem.Application.Services
{
    public class DistributorService : IService<Distributor>
    {
        private readonly IDistributorRepository _repository;

        public DistributorService(IDistributorRepository repository)
        {
            _repository = repository;
        }

        public List<Distributor> GetAll() => _repository.GetAll();

        public Distributor? Read(int id) => _repository.Read(id);

        public void Create(Distributor distributor)
        {
            var errors = DistributorValidation.Validate(distributor).ToList();
            if (errors.Any())
            {
                throw new System.ArgumentException(
                    $"Errores de validacion: {string.Join(", ", errors.Select(e => e.ToString()))}");
            }
            _repository.Create(distributor);
        }

        public void Update(Distributor distributor)
        {
            var errors = DistributorValidation.Validate(distributor).ToList();
            if (errors.Any())
            {
                throw new System.ArgumentException(
                    $"Errores de validacion: {string.Join(", ", errors.Select(e => e.ToString()))}");
            }
            _repository.Update(distributor);
        }

        public void Delete(int id) => _repository.Delete(id);
    }
}
