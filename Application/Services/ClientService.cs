using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Application.Services
{
    public class ClientService
    {
        private readonly IClientRepository _repository;

        public ClientService(IClientRepository repository)
        {
            _repository = repository;
        }

        public List<Client> GetAll() => _repository.GetAll();
        public Client? Read(int id) => _repository.Read(id);
        public void Create(Client client)
        {
            var errors = BookstoreManagementSystem.Domain.Validations.ClientValidation.Validate(client);
            if (errors != null && System.Linq.Enumerable.Any(errors))
                throw new ValidationException(errors);

            BookstoreManagementSystem.Domain.Validations.ClientValidation.Normalize(client);
            _repository.Create(client);
        }

        public void Update(Client client)
        {
            var errors = BookstoreManagementSystem.Domain.Validations.ClientValidation.Validate(client);
            if (errors != null && System.Linq.Enumerable.Any(errors))
                throw new ValidationException(errors);

            BookstoreManagementSystem.Domain.Validations.ClientValidation.Normalize(client);
            _repository.Update(client);
        }
        public void Delete(int id) => _repository.Delete(id);
    }
}
