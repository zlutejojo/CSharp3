using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Models;
using ToDoList.WebApi;

namespace ToDoList.Test;

public class DeleteTests : IDisposable
{
    ToDoItemsController controller;
    [Fact]
    public void Delete_DeletedItem_ReturnsOk()
    {
        // Arrange
        controller = new ToDoItemsController();
        ToDoItem todoItem = new ToDoItem
        {
            ToDoItemId = 1,
            Name = "Utři nádobí",
            Description = "Utři talíře a příbory",
            IsCompleted = true
        };

        // Act
        controller = new ToDoItemsController();
        controller.AddItemToStorage(todoItem);

        IActionResult actionResult = controller.DeleteById(1);
        var getResult = controller.Read();
        var value = getResult.GetValue();

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(actionResult);
        Assert.Equal(204, noContentResult.StatusCode);
    }

    //mazání pomocí reflexe - vyčištění statického seznamu items v ToDoItemsController
    public void Dispose()
    {
        var field = typeof(ToDoItemsController).GetField("items", BindingFlags.NonPublic | BindingFlags.Static);

        if (field != null)
        {
            // Získáme hodnotu pole (což je náš List<ToDoItem>)
            var list = field.GetValue(null) as List<ToDoItem>;

            // Vyčistíme seznam kompletně
            list?.Clear();
        }
    }
}
