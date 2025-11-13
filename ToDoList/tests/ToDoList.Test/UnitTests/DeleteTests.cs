using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using ToDoList.Domain.Models;
using ToDoList.Persistence;
using ToDoList.Persistence.Repositories;
using ToDoList.WebApi;


namespace ToDoList.Test.UnitTests;

public class DeleteTests
{
    private readonly IRepository<ToDoItem> repositoryMock;
    private readonly ToDoItemsController controller;

    public DeleteTests()
    {
        repositoryMock = Substitute.For<IRepository<ToDoItem>>();
        controller = new ToDoItemsController(repositoryMock);
    }

    [Fact]
    public void Delete_DeleteByIdValidItemId_ReturnsNoContent()
    {
        // Arrange
        int existingId = 1;
        var itemToDelete = new ToDoItem { ToDoItemId = existingId, Name = "Položka ke smazání", Description = "Tato položka bude smazána", IsCompleted = false };
        repositoryMock.GetById(existingId).Returns(itemToDelete);
        // Act
        IActionResult actionResult = controller.DeleteById(itemToDelete.ToDoItemId);

        // Assert
        repositoryMock.Received(1).Delete(existingId);

        var noContentResult = Assert.IsType<NoContentResult>(actionResult);
        Assert.Equal(204, noContentResult.StatusCode);
    }

    [Fact]
    public void Delete_DeleteByIdInvalidItemId_ReturnsNotFound()
    {
        // Arrange
        // předpokládám, že v seznamu není žádná položka s tímto ID
        int nonExistentId = 99999;
        repositoryMock.GetById(nonExistentId).Returns((ToDoItem)null);

        // Act
        var actionResult = controller.DeleteById(nonExistentId);

        // Assert
        repositoryMock.Received(1).GetById(nonExistentId);
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public void Delete_DeleteByIdUnhandledException_ReturnsInternalServerError()
    {
        // Arrange
        var exceptionMessage = "Database connection failed";
        int existingId = 1;
        var itemToUpdate = new ToDoItem { ToDoItemId = existingId, Name = "Test Item", Description = "Test Description", IsCompleted = false };
        repositoryMock.GetById(existingId).Returns(itemToUpdate);
        repositoryMock.When(x => x.Delete(existingId))
                      .Do(call => { throw new Exception(exceptionMessage); });

        // Act
        var actionResult = controller.DeleteById(existingId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(500, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Contains(exceptionMessage, problemDetails.Detail);
    }
}
