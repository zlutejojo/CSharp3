namespace ToDoList.Test;

using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Models;
using ToDoList.WebApi;

public class GetTests : IDisposable
{
    ToDoItemsController controller;
    ToDoItem todoItem1;
    ToDoItem todoItem2;
    [Fact]
    public void Get_AllItems_ReturnsAllItems()
    {
        // Arrange
        // todoItem1 = new ToDoItem
        // {
        //     ToDoItemId = 1,
        //     Name = "Udělej nákup",
        //     Description = "Kup rohlíky, maso, šunku",
        //     IsCompleted = false
        // };
        // todoItem2 = new ToDoItem
        // {
        //     ToDoItemId = 2,
        //     Name = "Umyj nádobí",
        //     Description = "Umyj talíře a příbory",
        //     IsCompleted = true
        // };
        // controller = new ToDoItemsController();
        // controller.AddItemToStorage(todoItem1);
        // controller.AddItemToStorage(todoItem2);

        // // Act
        // var result = controller.Read();
        // var value = result.GetValue();

        // // Assert
        // Assert.NotNull(value);

        // var firstToDo = value.First();
        // Assert.Equal(todoItem1.ToDoItemId, firstToDo.Id);
        // Assert.Equal(todoItem1.Name, firstToDo.Name);
        // Assert.Equal(todoItem1.Description, firstToDo.Description);
        // Assert.Equal(todoItem1.IsCompleted, firstToDo.IsCompleted);
    }

    public void Dispose()
    {
        controller.RemoveItemFromStorage(todoItem1);
        controller.RemoveItemFromStorage(todoItem2);
    }
}
