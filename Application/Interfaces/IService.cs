using System.Collections.Generic;

namespace BookstoreManagementSystem.Application.Interfaces
{
    public interface IService<T>
    {
        List<T> GetAll();
        T? Read(int id);
        void Create(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
