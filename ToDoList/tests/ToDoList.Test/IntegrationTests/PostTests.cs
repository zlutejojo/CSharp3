using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.DTOs;
using ToDoList.Persistence;
using ToDoList.WebApi;


namespace ToDoList.Test.IntegrationTests;

public class PostTests : IDisposable
{
    private readonly ToDoItemsContext _context;
    private readonly ToDoItemsController _controller;

    public PostTests()
    {
        _context = new ToDoItemsContext("Data Source=../../../IntegrationTests/data/localdb_test.db");
        _controller = new ToDoItemsController(_context);
    }

    [Fact]
    public void Post_CreateItem_ReturnsCreatedResponse()
    {
        // Arrange
        var request = new ToDoItemCreateRequestDto("Utři prach", "utři prach z poliček", false);

        // Act
        var actionResult = _controller.Create(request);
        var getResult = _controller.Read();

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal(201, createdResult.StatusCode);
        
        // Zkontrolujeme, že data položky v seznamu odpovídají tomu, co jsme vytvořili
        var okResult = Assert.IsType<OkObjectResult>(getResult.Result);
        var returnedList = Assert.IsAssignableFrom<IEnumerable<ToDoItemGetResponseDto>>(okResult.Value);

        // Zkontrolujeme, že data položky v seznamu odpovídají tomu, co jsme vytvořili
        Assert.Equal(request.Name, returnedList.First().Name);
        Assert.Equal(request.Description, returnedList.First().Description);
        Assert.Equal(request.IsCompleted, returnedList.First().IsCompleted);
        // Ověření, že bylo vygenerováno nějaké ID
        Assert.True(returnedList.First().Id > 0);
    }
    //mazání pomocí reflexe - vyčištění statického seznamu items v ToDoItemsController
    public void Dispose()
    {
        try
        {
            _context.ToDoItems.RemoveRange(_context.ToDoItems);
            _context.SaveChanges();
            //Reset ID počítače pro další testy
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
