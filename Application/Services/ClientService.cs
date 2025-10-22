using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Validations;
using System.Linq;
using BookstoreManagementSystem.Domain.Interfaces;

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

        public Result<bool> CreateResult(Client client)
        {
            var list = ClientValidation.Validate(client).ToList();
            if (list.Any())
            {
                var summary = string.Join("; ", list.Select(e => $"{e.Field}: {e.Message}"));
                return Result<bool>.Fail(summary);
            }
            ClientValidation.Normalize(client);
            _repository.Create(client);
            return Result<bool>.Ok(true);
        }

        public void Update(Client client)
        {
            var errors = ClientValidation.Validate(client);
            if (errors != null && errors.Any())
                throw new ValidationException(errors);

            ClientValidation.Normalize(client);
            _repository.Update(client);
        }

        public Result<bool> UpdateResult(Client client)
        {
            var list = ClientValidation.Validate(client).ToList();
            if (list.Any())
            {
                var summary = string.Join("; ", list.Select(e => $"{e.Field}: {e.Message}"));
                return Result<bool>.Fail(summary);
            }
            ClientValidation.Normalize(client);
            _repository.Update(client);
            return Result<bool>.Ok(true);
        }
        public void Delete(Guid id) => _repository.Delete(id);
    }
}
