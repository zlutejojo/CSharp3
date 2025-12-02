using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Models;
using ToDoList.Persistence;
using ToDoList.Persistence.Repositories;
using ToDoList.WebApi;


namespace ToDoList.Test.IntegrationTests;

public class DeleteTests : IAsyncLifetime
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
    public async Task Delete_DeletedItem_ReturnsOk()
    {
        // Arrange
        var itemToDelete = new ToDoItem { Name = "Položka ke smazání", Description = "Tato položka bude smazána", IsCompleted = false, Category = "Tato položka bude smazána" };
        context.ToDoItems.Add(itemToDelete);
        await context.SaveChangesAsync();

        // Act
        IActionResult actionResult = await controller.DeleteById(itemToDelete.ToDoItemId);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(actionResult);
        Assert.Equal(204, noContentResult.StatusCode);

        var itemInDb = await context.ToDoItems.FindAsync(itemToDelete.ToDoItemId);
        Assert.Null(itemInDb);
    }

    [Fact]
    public async Task Delete_NonExistentItem_ReturnsNotFound()
    {
        // Arrange
        // předpokládám, že v seznamu není žádná položka s tímto ID
        int nonExistentId = 99999;

        // Act
        var actionResult = await controller.DeleteById(nonExistentId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        try
        {
            context.ToDoItems.RemoveRange(context.ToDoItems);
            await context.SaveChangesAsync();

            // Resetujeme identity counter (auto-increment), aby další testy začínaly s ID 1
            await context.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name='ToDoItems'");
        }
        catch (Exception)
        {

        }
        finally
        {
            await context.DisposeAsync();
        }
    }
}
