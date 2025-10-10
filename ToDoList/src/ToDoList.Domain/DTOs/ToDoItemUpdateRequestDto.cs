namespace ToDoList.Domain.DTOs;

using ToDoList.Domain.Models;

public record ToDoItemUpdateRequestDto(string Name, string Description, bool IsCompleted)
{

}

