namespace ToDoList.Test.IntegrationTests;

using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.Persistence;
using ToDoList.WebApi;

public class GetTests : IDisposable
{
    private readonly ToDoItemsContext _context;
    private readonly ToDoItemsController _controller;

    public ToDoItem todoItem1;
    public ToDoItem todoItem2;

    public GetTests()
    {
        _context = new ToDoItemsContext("Data Source=../../../IntegrationTests/data/localdb_test.db");
        _context.Database.EnsureCreated();

        _controller = new ToDoItemsController(_context);
    }

    [Fact]
    public void Get_AllItems_ReturnsAllItems()
    {
        // Arrange
        todoItem1 = new ToDoItem
        {

            ToDoItemId = 1,
            Name = "Udělej nákup",
            Description = "Kup rohlíky, maso, šunku",
            IsCompleted = false
        };
        todoItem2 = new ToDoItem
        {
            ToDoItemId = 2,
            Name = "Umyj nádobí",
            Description = "Umyj talíře a příbory",
            IsCompleted = true
        };
        _context.ToDoItems.Add(todoItem1);
        _context.ToDoItems.Add(todoItem2);
        _context.SaveChanges();

        // Act
        var actionResult = _controller.Read();


        // Assert
        // ověření, že akce vrátila správný typ odpovědi OK 200, zároveň přetypuje na OkObjectResult
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedItems = Assert.IsAssignableFrom<IEnumerable<ToDoItemGetResponseDto>>(okResult.Value);

        var itemsList = returnedItems.ToList();
        Assert.Equal(2, itemsList.Count);

        var firstItem = itemsList.First();
        Assert.Equal(todoItem1.ToDoItemId, firstItem.Id);
        Assert.Equal(todoItem1.Name, firstItem.Name);
        Assert.Equal(todoItem1.Description, firstItem.Description);
        Assert.Equal(todoItem1.IsCompleted, firstItem.IsCompleted);

        var secondItem = itemsList.Last();
        Assert.Equal(todoItem2.Name, secondItem.Name);
        Assert.Equal(todoItem2.Description, secondItem.Description);
        Assert.Equal(todoItem2.IsCompleted, secondItem.IsCompleted);
    }

    public void Dispose()
    {
        try
        {
            _context.ToDoItems.RemoveRange(_context.ToDoItems);
            _context.SaveChanges();
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
