using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;

namespace BookstoreManagementSystem.Application.Interfaces
{
    public interface ICategoryRepository
    {
        void Create(Category category);
        Category? Read(int id);
        void Update(Category category);
        void Delete(int id);
        List<Category> GetAll();
    }
}
