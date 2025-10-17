using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.WebApi;

namespace ToDoList.Test;

public class PutTests : IDisposable
{
    ToDoItemsController controller;
    [Fact]
    public void Put_UpdateItem_ReturnsCreatedResponse()
    {
        // Arrange
        controller = new ToDoItemsController();
        var request = new ToDoItemUpdateRequestDto("Vyžehli", "použij napařovací žehličku", false);

        // Act
        ToDoItem todoItem1 = new ToDoItem
        {
            ToDoItemId = 1,
            Name = "Běž ven",
            Description = "Ujdi aspoň 5 km",
            IsCompleted = false
        };
        controller.AddItemToStorage(todoItem1);
        IActionResult actionResult = controller.UpdateById(1, request);
        var getResult = controller.Read();
        var value = getResult.GetValue();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(200, okResult.StatusCode);

        Assert.Equal(request.Name, value.First().Name);
        Assert.Equal(request.Description, value.First().Description);
        Assert.Equal(request.IsCompleted, value.First().IsCompleted);
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
