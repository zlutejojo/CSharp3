using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.Persistence.Repositories;
using ToDoList.WebApi;


namespace ToDoList.Test.UnitTests;

public class PostTests
{

    [Fact]
    public void Post_CreateItem_ReturnsCreatedResponse()
    {
        // Arrange
        IRepository<ToDoItem> repositoryMock = Substitute.For<IRepository<ToDoItem>>();
        ToDoItemsController controller = new ToDoItemsController(repositoryMock);
        ToDoItemCreateRequestDto request = new ToDoItemCreateRequestDto(
            Name: "Uvař oběd",
            Description: "Udělej pečené kuře s rýží",
            IsCompleted: false
        );
        //připravíme doménový objekt
        ToDoItem itemInDb = new ToDoItem
        {
            ToDoItemId = 1,
            Name = request.Name,
            Description = request.Description,
            IsCompleted = request.IsCompleted
        };
        //nastavení mocku, aby při volání metody GetAll vracel seznam s jednou položkou
        repositoryMock.GetAll().Returns(new List<ToDoItem> { itemInDb });

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

}
