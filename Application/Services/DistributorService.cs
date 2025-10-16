using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;
using BookstoreManagementSystem.Domain.Validations;
using System.Linq;

namespace BookstoreManagementSystem.Application.Services
{
    public class DistributorService: IDistributorService
    {
        private readonly IDistributorRepository _repository;

        public DistributorService(IDistributorRepository repository)
        {
            _repository = repository;
        }

        public List<Distributor> GetAll() => _repository.GetAll();
        public Distributor? Read(Guid id) => _repository.Read(id);
        public void Create(Distributor distributor)
        {
            var errors = DistributorValidation.Validate(distributor);
            if (errors != null && errors.Any())
                throw new ValidationException(errors);

            DistributorValidation.Normalize(distributor);
            _repository.Create(distributor);
        }

        public void Update(Distributor distributor)
        {
            var errors = DistributorValidation.Validate(distributor);
            if (errors != null && errors.Any())
                throw new ValidationException(errors);

            DistributorValidation.Normalize(distributor);
            _repository.Update(distributor);
        }
        public void Delete(Guid id) => _repository.Delete(id);
    }
}
