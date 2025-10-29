using ServiceProducts.Domain.Models;

namespace ServiceProducts.Domain.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        Product? Read(Guid id);
        void Create(Product product);
        void Update(Product product);
        void Delete(Guid id);
    }
}
