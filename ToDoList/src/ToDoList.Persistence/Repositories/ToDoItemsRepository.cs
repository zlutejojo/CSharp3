using System;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;

namespace ToDoList.Persistence.Repositories;

public class ToDoItemsRepository : IRepository<ToDoItem>
{
    private readonly ToDoItemsContext context;

    public ToDoItemsRepository(ToDoItemsContext context)
    {
        this.context = context;
    }
    public void Create(ToDoItem item)
    {
        context.ToDoItems.Add(item);
        context.SaveChanges();
    }

    public IEnumerable<ToDoItem> GetAll()
    {
        return context.ToDoItems.ToList();
    }

    public ToDoItem GetById(int id)
    {
        // Najde položku podle jejího primárního klíče
        return context.ToDoItems.Find(id);
    }

    public void Update(ToDoItem entity)
    {
        context.ToDoItems.Update(entity);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var itemToDelete = context.ToDoItems.Find(id);
        if (itemToDelete != null)
        {
            context.ToDoItems.Remove(itemToDelete);
            context.SaveChanges();
        }
    }


}
