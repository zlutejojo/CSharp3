using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.Persistence;
using ToDoList.WebApi;


namespace ToDoList.Test.IntegrationTests;

public class PutTests : IDisposable
{
    private readonly ToDoItemsContext _context;
    private readonly ToDoItemsController _controller;

    public PutTests()
    {
        _context = new ToDoItemsContext("Data Source=../../../IntegrationTests/data/localdb_test.db");
        _controller = new ToDoItemsController(_context);
    }

    [Fact]
    public void Put_UpdateItem_ReturnsCreatedResponse()
    {
        // Arrange
        var originalItem = new ToDoItem { Name = "Vyper", Description = "Vyper barevné prádlo" };
        _context.ToDoItems.Add(originalItem);
        _context.SaveChanges();

        var request = new ToDoItemUpdateRequestDto("Vyper", "Vyper bílé prádlo", true);

        // Act
        var actionResult = _controller.UpdateById(originalItem.ToDoItemId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(200, okResult.StatusCode);

        var returnedDto = Assert.IsType<ToDoItemGetResponseDto>(okResult.Value);

        // Nyní ověřte vlastnosti vráceného DTO
        Assert.Equal(request.Name, returnedDto.Name);
        Assert.Equal(request.Description, returnedDto.Description);
        Assert.Equal(request.IsCompleted, returnedDto.IsCompleted);

    }

    [Fact]
    public void Put_UpdateNonExistentItem_ReturnsNotFound()
    {
        // Arrange
        var request = new ToDoItemUpdateRequestDto("Nic", "Nic", false);
        //předpokládám, že v seznamu není žádná položka s tímto ID
        int nonExistentId = 99999;

        // Act
        // Zavoláme metodu pro update s neexistujícím ID.
        IActionResult actionResult = _controller.UpdateById(nonExistentId, request);

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
