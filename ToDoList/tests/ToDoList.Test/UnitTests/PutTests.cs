
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.Persistence.Repositories;
using ToDoList.WebApi;


namespace ToDoList.Test.UnitTests;

public class PutTests
{
    private readonly IRepositoryAsync<ToDoItem> repositoryMock;
    private readonly ToDoItemsController controller;

    public PutTests()
    {
        repositoryMock = Substitute.For<IRepositoryAsync<ToDoItem>>();
        controller = new ToDoItemsController(repositoryMock);
    }

    [Fact]
    public async Task Put_UpdateByIdWhenItemUpdated_ReturnsOkResponse()
    {
        // Arrange
        int existingId = 1;
        var originalItem = new ToDoItem { ToDoItemId = existingId, Name = "Vyper", Description = "Vyper barevné prádlo", IsCompleted = false };

        repositoryMock.GetByIdAsync(existingId).Returns(originalItem);

        var request = new ToDoItemUpdateRequestDto("Vyper", "Vyper bílé prádlo", false, "domácí práce");

        // Act
        var actionResult = await controller.UpdateById(existingId, request);

        // Assert
        await repositoryMock.Received(1).UpdateAsync(originalItem);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(200, okResult.StatusCode);

        var returnedDto = Assert.IsType<ToDoItemGetResponseDto>(okResult.Value);

        Assert.Equal(request.Name, returnedDto.Name);
        Assert.Equal(request.Description, returnedDto.Description);
        Assert.Equal(request.IsCompleted, returnedDto.IsCompleted);
        Assert.Equal(request.Category, returnedDto.Category);

    }

    [Fact]
    public async Task Put_UpdateByIdWhenIdNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new ToDoItemUpdateRequestDto("Nic", "Nic", false, "Nic");
        int nonExistentId = 99999;
        repositoryMock.GetByIdAsync(nonExistentId).Returns(Task.FromResult<ToDoItem?>(null));

        // Act
        // Zavoláme metodu pro update s neexistujícím ID.
        IActionResult actionResult = await controller.UpdateById(nonExistentId, request);
        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
        Assert.Equal(404, notFoundResult.StatusCode);
        repositoryMock.DidNotReceive().UpdateAsync(Arg.Any<ToDoItem>());
    }

    [Fact]
    public async Task Put_UpdateByIdUnhandledException_ReturnsInternalServerError()
    {
        // Arrange
        var exceptionMessage = "Database connection failed";
        int existingId = 1;
        var itemToUpdate = new ToDoItem { ToDoItemId = existingId, Name = "Test Item", Description = "Test Description", IsCompleted = false };
        // Nejprve musíme simulovat, že položka byla nalezena
        repositoryMock.GetByIdAsync(existingId).Returns(itemToUpdate);
        // až potom nastavíme, že samotný update selže
        repositoryMock.When(x => x.UpdateAsync(Arg.Any<ToDoItem>()))
                      .Do(call => { throw new Exception(exceptionMessage); });

        // Act
        var actionResult = await controller.UpdateById(existingId, new ToDoItemUpdateRequestDto("Test", "Test", false, "Test"));
        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(500, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Contains(exceptionMessage, problemDetails.Detail);
    }
}
