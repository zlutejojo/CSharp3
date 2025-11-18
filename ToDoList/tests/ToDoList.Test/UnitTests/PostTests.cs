using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.Persistence.Repositories;
using ToDoList.WebApi;


namespace ToDoList.Test.UnitTests;

public class PostTests
{
    private readonly IRepository<ToDoItem> repositoryMock;
    private readonly ToDoItemsController controller;

    public PostTests()
    {
        repositoryMock = Substitute.For<IRepository<ToDoItem>>();
        controller = new ToDoItemsController(repositoryMock);
    }

    [Fact]
    public void Post_CreateValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        ToDoItemCreateRequestDto request = new ToDoItemCreateRequestDto(
            Name: "Uvař oběd",
            Description: "Udělej pečené kuře s rýží",
            IsCompleted: false
        );

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
    }

    [Fact]
    public void Post_CreateUnhandledException_ReturnsInternalServerError()
    {
        // Arrange
        var exceptionMessage = "Database connection failed";

        repositoryMock.When(x => x.Create(Arg.Any<ToDoItem>()))
                      .Do(call => { throw new Exception(exceptionMessage); });

        var controller = new ToDoItemsController(repositoryMock);
        var request = new ToDoItemCreateRequestDto("Ukol", "Popis ukolu", false);

        // Act
        var actionResult = controller.Create(request);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(500, objectResult.StatusCode);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Contains(exceptionMessage, problemDetails.Detail);
    }
}
