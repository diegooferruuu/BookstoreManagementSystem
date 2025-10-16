namespace BookstoreManagementSystem.Domain.Services
{
    public interface IDataBase<T>
    {
        void Create(T entity);
        void Update(T entity);
    T? Read(Guid id);
        void Delete(Guid id);
        List<T> GetAll();
    }
}

