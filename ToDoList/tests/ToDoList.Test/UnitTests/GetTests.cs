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
    public void Get_ReadWhenSomeItemAvailable_ReturnsOk()
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
        repositoryMock.Received(1).GetAll();

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

    [Fact]
    public void Get_ReadByIdWhenSomeItemAvailable_ReturnsOk()
    {
        // Arrange
        int existingId = 1;
        var item = new ToDoItem { ToDoItemId = existingId, Name = "Vyluxuj", Description = "Vyluxuj celý byt", IsCompleted = false };

        repositoryMock.GetById(existingId).Returns(item);

        // Act
        var actionResult = controller.ReadById(existingId);

        // Assert
        repositoryMock.Received(1).GetById(existingId);

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedDto = Assert.IsType<ToDoItemGetResponseDto>(okResult.Value);

        // C. Ověříme, že vrácená data jsou správná
        Assert.Equal(existingId, returnedDto.Id);
        Assert.Equal(item.Name, returnedDto.Name);
        Assert.Equal(item.Description, returnedDto.Description);
        Assert.Equal(item.IsCompleted, returnedDto.IsCompleted);
    }

    [Fact]
    public void Get_ReadWhenNoItemAvailable_ReturnsOkWithEmptyCollection()
    {
        // Arrange
        // Nastavíme mock, aby vrátil prázdný seznam
        repositoryMock.GetAll().Returns(new List<ToDoItem>());

        // Act
        var actionResult = controller.Read();

        // Assert
        repositoryMock.Received(1).GetAll();
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        // Ověříme, že vrácená kolekce je prázdná
        var returnedItems = Assert.IsAssignableFrom<IEnumerable<ToDoItemGetResponseDto>>(okResult.Value);
        Assert.Empty(returnedItems);
    }

    [Fact]
    public void Get_ReadByIdWhenItemIsNull_ReturnsNotFound()
    {
        // Arrange
        int nonExistentId = 999;

        repositoryMock.GetById(nonExistentId).Returns((ToDoItem)null);

        // Act
        var actionResult = controller.ReadById(nonExistentId);

        // Assert
        repositoryMock.Received(1).GetById(nonExistentId);
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    public void Get_ReadUnhandledException_ReturnsInternalServerError()
    {
        // Arrange
        var exceptionMessage = "Database connection failed";

        // Nastavíme mock, aby vyvolal výjimku při volání metody GetAll()
        repositoryMock.When(x => x.GetAll())
                      .Do(call => { throw new Exception(exceptionMessage); });

        // Act
        var actionResult = controller.Read();

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(500, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Contains(exceptionMessage, problemDetails.Detail);
    }

    [Fact]
    public void Get_ReadByIdUnhandledException_ReturnsInternalServerError()
    {
        // Arrange
        var exceptionMessage = "Database connection failed";

        // Nastavíme mock, aby vyvolal výjimku při volání metody GetById()
        repositoryMock.When(x => x.GetById(Arg.Any<int>()))
                      .Do(call => { throw new Exception(exceptionMessage); });

        // Act
        var actionResult = controller.ReadById(1);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(500, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Contains(exceptionMessage, problemDetails.Detail);
    }
}

