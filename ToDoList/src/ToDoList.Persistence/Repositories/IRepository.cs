namespace ToDoList.Persistence.Repositories;

public interface IRepository<T>
where T : class
{
    public void Create(T item);
    IEnumerable<T> GetAll();
    T GetById(int id);
    void Update(T entity);
    void Delete(int id);
}
