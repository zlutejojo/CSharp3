using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Models;
using ToDoList.WebApi;

namespace ToDoList.Test;

public class DeleteTests : IDisposable
{
    private readonly ToDoItemsController _controller;

    public DeleteTests()
    {
        _controller = new ToDoItemsController();
    }
    [Fact]
    public void Delete_DeletedItem_ReturnsOk()
    {
        // Arrange
        ToDoItem todoItem = new ToDoItem
        {
            ToDoItemId = 1,
            Name = "Utři nádobí",
            Description = "Utři talíře a příbory",
            IsCompleted = true
        };

        // Act
        _controller.AddItemToStorage(todoItem);
        IActionResult actionResult = _controller.DeleteById(1);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(actionResult);
        Assert.Equal(204, noContentResult.StatusCode);
    }

    [Fact]
    public void Delete_NonExistentItem_ReturnsNotFound()
    {
        // Arrange
        // předpokládám, že v seznamu není žádná položka s tímto ID
        int nonExistentId = 99999;

        // Act
        var actionResult = _controller.DeleteById(nonExistentId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
        Assert.Equal(404, notFoundResult.StatusCode);
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
