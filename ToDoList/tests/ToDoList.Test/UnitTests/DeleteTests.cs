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
    public void Delete_DeletedItem_ReturnsOk()
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
    public void Delete_NonExistentItem_ReturnsNotFound()
    {
        // Arrange
        // předpokládám, že v seznamu není žádná položka s tímto ID
        int nonExistentId = 99999;

        // Act
        var actionResult = controller.DeleteById(nonExistentId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
}
