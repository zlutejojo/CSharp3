using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.Persistence.Repositories;
using ToDoList.WebApi;


namespace ToDoList.Test.UnitTests;

public class PostTests
{
    private readonly IRepositoryAsync<ToDoItem> repositoryMock;
    private readonly ToDoItemsController controller;

    public PostTests()
    {
        repositoryMock = Substitute.For<IRepositoryAsync<ToDoItem>>();
        controller = new ToDoItemsController(repositoryMock);
    }

    [Fact]
    public async Task Post_CreateValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        ToDoItemCreateRequestDto request = new ToDoItemCreateRequestDto(
            Name: "Uvař oběd",
            Description: "Udělej pečené kuře s rýží",
            IsCompleted: false,
            Category: "Domácí práce"
        );

        // Act
        var result = await controller.Create(request);
        IActionResult actionResult = result.Result;

        // Assert
        // ověření, že metoda Create byla zavolána jednou s libovolnou položkou ToDoItem
        await repositoryMock.Received(1).CreateAsync(Arg.Any<ToDoItem>());
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);

        var returnedDto = Assert.IsType<ToDoItemGetResponseDto>(createdResult.Value);

        // ověříme, že odpověď obsahuje správná data
        Assert.Equal(request.Name, returnedDto.Name);
        Assert.Equal(request.Description, returnedDto.Description);
        Assert.Equal(request.IsCompleted, returnedDto.IsCompleted);
        Assert.Equal(request.Category, returnedDto.Category);
    }

    [Fact]
    public async Task Post_CreateUnhandledException_ReturnsInternalServerError()
    {
        // Arrange
        var exceptionMessage = "Database connection failed";

        repositoryMock.When(x => x.CreateAsync(Arg.Any<ToDoItem>()))
                      .Do(call => { throw new Exception(exceptionMessage); });

        var controller = new ToDoItemsController(repositoryMock);
        var request = new ToDoItemCreateRequestDto("Ukol", "Popis ukolu", false, "Kategorie");

        // Act
        var result = await controller.Create(request);
        var actionResult = result.Result;

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(500, objectResult.StatusCode);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Contains(exceptionMessage, problemDetails.Detail);
    }
}
