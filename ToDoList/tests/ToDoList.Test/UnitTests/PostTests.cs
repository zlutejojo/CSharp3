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

        // Assert
        // ověření, že metoda Create byla zavolána jednou s libovolnou položkou ToDoItem
        repositoryMock.Received(1).Create(Arg.Any<ToDoItem>());
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);

        var returnedDto = Assert.IsType<ToDoItemGetResponseDto>(createdResult.Value);

        // ověříme, že odpověď obsahuje správná data
        Assert.Equal(request.Name, returnedDto.Name);
        Assert.Equal(request.Description, returnedDto.Description);
        Assert.Equal(request.IsCompleted, returnedDto.IsCompleted);
        // Ověření, že bylo vygenerováno nějaké ID
        Assert.True(returnedDto.Id > 0);
    }

}
