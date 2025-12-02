using System;

namespace ToDoList.Persistence.Repositories;

public interface IRepositoryAsync<T>
where T : class
{
    Task CreateAsync(T item);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
