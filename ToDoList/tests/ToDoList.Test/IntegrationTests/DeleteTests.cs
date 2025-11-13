using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Models;
using ToDoList.Persistence;
using ToDoList.Persistence.Repositories;
using ToDoList.WebApi;


namespace ToDoList.Test.IntegrationTests;

public class DeleteTests : IDisposable
{
    private readonly ToDoItemsContext context;
    private readonly ToDoItemsController controller;
    private readonly ToDoItemsRepository repository;

    public DeleteTests()
    {
        context = new ToDoItemsContext("Data Source=../../../IntegrationTests/data/localdb_test.db");
        repository = new ToDoItemsRepository(context);
        controller = new ToDoItemsController(repository);
    }

    [Fact]
    public void Delete_DeletedItem_ReturnsOk()
    {
        // Arrange
        var itemToDelete = new ToDoItem { Name = "Položka ke smazání", Description = "Tato položka bude smazána", IsCompleted = false };
        context.ToDoItems.Add(itemToDelete);
        context.SaveChanges();

        // Act
        IActionResult actionResult = controller.DeleteById(itemToDelete.ToDoItemId);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(actionResult);
        Assert.Equal(204, noContentResult.StatusCode);

        var itemInDb = context.ToDoItems.Find(itemToDelete.ToDoItemId);
        Assert.Null(itemInDb);
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

    //mazání pomocí reflexe - vyčištění statického seznamu items v ToDoItemsController
    public void Dispose()
    {
        try
        {
            context.ToDoItems.RemoveRange(context.ToDoItems);
            context.SaveChanges();

            // Resetujeme identity counter (auto-increment), aby další testy začínaly s ID 1
            context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name='ToDoItems'");
        }
        catch (Exception)
        {

        }
        finally
        {
            context?.Dispose();
        }
    }
}
