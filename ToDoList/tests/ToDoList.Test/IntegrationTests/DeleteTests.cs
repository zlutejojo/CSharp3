using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Models;
using ToDoList.Persistence;
using ToDoList.WebApi;
using Xunit;

namespace ToDoList.Test.IntegrationTests;

public class DeleteTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ToDoItemsContext _context;
    private readonly ToDoItemsController _controller;

    public DeleteTests()
    {
        _context = new ToDoItemsContext("Data Source=../../../IntegrationTests/data/localdb_test.db");
        _context.Database.EnsureCreated();

        _controller = new ToDoItemsController(_context);
    }

    [Fact]
    public void Delete_DeletedItem_ReturnsOk()
    {
        // Arrange
        var itemToDelete = new ToDoItem { Name = "Položka ke smazání", Description = "Tato položka bude smazána", IsCompleted = false };
        _context.ToDoItems.Add(itemToDelete);
        _context.SaveChanges();

        // Act
        IActionResult actionResult = _controller.DeleteById(itemToDelete.ToDoItemId);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(actionResult);
        Assert.Equal(204, noContentResult.StatusCode);

        var itemInDb = _context.ToDoItems.Find(itemToDelete.ToDoItemId);
        Assert.Null(itemInDb);
    }

    [Fact]
    public void Delete_NonExistentItem_ReturnsNotFound()
    {
        // Arrange
        // předpokládám, že v seznamu není žádná položka s tímto ID
        int nonExistentId = 99999;

        // Act
        var actionResult = _controller.DeleteById(nonExistentId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    //mazání pomocí reflexe - vyčištění statického seznamu items v ToDoItemsController
    public void Dispose()
    {
        try
        {
            _context.ToDoItems.RemoveRange(_context.ToDoItems);
            _context.SaveChanges();

            // Resetujeme identity counter (auto-increment), aby další testy začínaly s ID 1
            _context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name='ToDoItems'");
        }
        catch (Exception)
        {

        }
        finally
        {
            _context?.Dispose();
        }
    }
}
