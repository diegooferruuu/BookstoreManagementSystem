namespace BookstoreManagementSystem.Services
{
    public interface IDataBase<T>
    {
        void Create(T entity);
        void Update(T entity);
        T Read(int id);
        void Delete(int id);
    }
}
