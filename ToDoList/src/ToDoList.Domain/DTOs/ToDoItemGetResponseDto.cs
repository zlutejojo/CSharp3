using System;
using ToDoList.Domain.Models;

namespace ToDoList.Domain.DTOs;

public record ToDoItemGetResponseDto(int Id, string Name, string Description, bool IsCompleted) //let client know the Id
{
    public static ToDoItemGetResponseDto FromDomain(ToDoItem item) => new(item.ToDoItemId, item.Name, item.Description, item.IsCompleted);
}
