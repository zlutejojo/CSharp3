using System;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;

namespace ToDoList.Persistence.Repositories;

public class ToDoItemsRepository : IRepositoryAsync<ToDoItem>
{
    private readonly ToDoItemsContext context;

    public ToDoItemsRepository(ToDoItemsContext context)
    {
        this.context = context;
    }
    public async Task CreateAsync(ToDoItem item)
    {
        await context.ToDoItems.AddAsync(item);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ToDoItem>> GetAllAsync()
    {
        return await context.ToDoItems.ToListAsync();
    }

    public async Task<ToDoItem?> GetByIdAsync(int id)
    {
        // Najde položku podle jejího primárního klíče
        return await context.ToDoItems.FindAsync(id);
    }

    public async Task UpdateAsync(ToDoItem entity)
    {
        context.ToDoItems.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var itemToDelete = await context.ToDoItems.FindAsync(id);
        if (itemToDelete != null)
        {
            context.ToDoItems.Remove(itemToDelete);
            await context.SaveChangesAsync();
        }
    }


}
