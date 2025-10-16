using BookstoreManagementSystem.Domain.Models;

namespace BookstoreManagementSystem.Domain.Interfaces
{
    public interface IProductService
    {
        public List<Product> GetAll();
        public Product? Read(Guid id);
        public void Create(Product product);
        public void Update(Product product);
        public void Delete(Guid id);

    }
}
