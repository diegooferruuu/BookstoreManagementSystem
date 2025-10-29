using ServiceProducts.Domain.Models;

namespace ServiceProducts.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        Category? Read(Guid id);
    }
}
