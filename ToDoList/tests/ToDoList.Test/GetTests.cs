using System;
using ToDoList.Domain.Models;
using ToDoList.WebApi;
namespace ToDoList.Test;

public class GetTests
{
    [Fact]
    public void Get_AllItems_ReturnsAllItems()
    {
        // Arrange
        var ToDoItem = new ToDoItem
        {
            ToDoItemId = 1,
            Name = "Udelej",
            Description = "Udelej to poradne",
            IsCompleted = false
        };
        var controller = new ToDoItemsController();
        

        // Act
        var result = controller.Read();
        var value = result;


        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

}
