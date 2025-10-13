namespace ToDoList.Domain.DTOs;

public record ToDoItemCreateRequestDto(string Name, string Description, bool IsCompleted) //id is generated
{
    public ToDoItem ToDomain() => new() { Name = Name, Description = Description, IsCompleted = IsCompleted };
}
