using System;
using ToDoList.Frontend.Models;

namespace ToDoList.Frontend.Clients;

public interface IToDoItemsClient
{
    public Task<List<ToDoItemView>> ReadItemsAsync();
}
