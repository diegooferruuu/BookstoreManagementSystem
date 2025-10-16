using BookstoreManagementSystem.Domain.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Validations;
using System.Linq;

namespace BookstoreManagementSystem.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _repository;

        public ClientService(IClientRepository repository)
        {
            _repository = repository;
        }

        public List<Client> GetAll() => _repository.GetAll();
        public Client? Read(Guid id) => _repository.Read(id);
        public void Create(Client client)
        {
            var errors = ClientValidation.Validate(client);
            if (errors != null && errors.Any())
                throw new ValidationException(errors);

            ClientValidation.Normalize(client);
            _repository.Create(client);
        }

        public void Update(Client client)
        {
            var errors = ClientValidation.Validate(client);
            if (errors != null && errors.Any())
                throw new ValidationException(errors);

            ClientValidation.Normalize(client);
            _repository.Update(client);
        }
        public void Delete(Guid id) => _repository.Delete(id);
    }
}
