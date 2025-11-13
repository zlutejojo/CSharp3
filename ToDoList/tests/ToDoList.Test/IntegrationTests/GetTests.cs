namespace ToDoList.Test.IntegrationTests;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.Persistence;
using ToDoList.Persistence.Repositories;
using ToDoList.WebApi;

public class GetTests : IDisposable
{
    private readonly ToDoItemsContext context;
    private readonly ToDoItemsController controller;
    private readonly ToDoItemsRepository repository;

    public ToDoItem todoItem1;
    public ToDoItem todoItem2;

    public GetTests()
    {
        context = new ToDoItemsContext("Data Source=../../../IntegrationTests/data/localdb_test.db");
        repository = new ToDoItemsRepository(context);
        controller = new ToDoItemsController(repository);
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
        context.ToDoItems.Add(todoItem1);
        context.ToDoItems.Add(todoItem2);
        context.SaveChanges();

        // Act
        var actionResult = controller.Read();


        // Assert
        // ověření, že akce vrátila správný typ odpovědi OK 200, zároveň přetypuje na OkObjectResult
        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(200, objectResult.StatusCode);

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
            context.ToDoItems.RemoveRange(context.ToDoItems);
            context.SaveChanges();
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
