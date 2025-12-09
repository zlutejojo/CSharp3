using System;
using ToDoList.Frontend.Models;

namespace ToDoList.Frontend.Clients;

public interface IToDoItemsClient
{
    public Task<List<ToDoItemView>> ReadItemsAsync();
    public Task<ToDoItemView?> ReadItemByIdAsync(int itemId);
    public Task UpdateItemAsync(ToDoItemView item);
    public Task DeleteItemAsync(int itemId);

    public Task CreateItemAsync(ToDoItemView item);
}
