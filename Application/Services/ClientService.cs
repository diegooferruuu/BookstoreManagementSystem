using BookstoreManagementSystem.Application.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using BookstoreManagementSystem.Domain.Validations;
using System.Linq;

namespace BookstoreManagementSystem.Application.Services
{
    public class ClientService : IService<Client>
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
            var errors = ClientValidation.Validate(client).ToList();
            if (errors.Any())
            {
                throw new System.ArgumentException(
                    $"Errores de validacion: {string.Join(", ", errors.Select(e => e.ToString()))}");
            }
            _repository.Create(client);
        }

        public void Update(Client client)
        {
            var errors = ClientValidation.Validate(client).ToList();
            if (errors.Any())
            {
                throw new System.ArgumentException(
                    $"Errores de validacion: {string.Join(", ", errors.Select(e => e.ToString()))}");
            }
            _repository.Update(client);
        }

        public void Delete(int id) => _repository.Delete(id);
    }
}
