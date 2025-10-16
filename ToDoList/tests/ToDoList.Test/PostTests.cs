using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.WebApi;


namespace ToDoList.Test;

public class PostTests : IDisposable
{
    ToDoItemsController controller;

    [Fact]
    public void Post_CreateItem_ReturnsCreatedResponse()
    {
        // Arrange
        controller = new ToDoItemsController();
        var request = new ToDoItemCreateRequestDto("Vyper", "vyper barevné oblečení", false);

        // Act
        IActionResult actionResult = controller.Create(request);

        var getResult = controller.Read();

        //ověření, zda zavolání metody Create přidalo položku do seznamu úkolů items
        // Zkontroluje, že výsledek je typu OkObjectResult a zároveň ho přetypuje (abych se dostala k hodnotě value)
        var okResult = Assert.IsType<OkObjectResult>(getResult.Result);
        // Zkontroluje, zda objekt uložený v okResult.Value je kompatibilní s typem IEnumerable<ToDoItemGetResponseDto> a zároveň ho přetypuje
        var returnedList = Assert.IsAssignableFrom<IEnumerable<ToDoItemGetResponseDto>>(okResult.Value);

        // Zkontrolujeme, že data položky v seznamu odpovídají tomu, co jsme vytvořili
        Assert.Equal(request.Name, returnedList.First().Name);
        Assert.Equal(request.Description, returnedList.First().Description);
        Assert.Equal(request.IsCompleted, returnedList.First().IsCompleted);
        // Ověření, že bylo vygenerováno nějaké ID
        Assert.True(returnedList.First().Id > 0);
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
