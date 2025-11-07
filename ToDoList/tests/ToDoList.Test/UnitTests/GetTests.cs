namespace ToDoList.Test.UnitTests;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.Persistence;
using ToDoList.Persistence.Repositories;
using ToDoList.WebApi;

public class GetTests
{
    private readonly IRepository<ToDoItem> repositoryMock;
    private readonly ToDoItemsController controller;

    public ToDoItem todoItem1;
    public ToDoItem todoItem2;

    public GetTests()
    {
        repositoryMock = Substitute.For<IRepository<ToDoItem>>();
        controller = new ToDoItemsController(repositoryMock);
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

        var items = new List<ToDoItem> { todoItem1, todoItem2 };
        repositoryMock.GetAll().Returns(items);

        // Act
        var actionResult = controller.Read();


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
}

