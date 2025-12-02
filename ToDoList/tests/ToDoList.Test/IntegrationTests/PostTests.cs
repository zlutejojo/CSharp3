using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.DTOs;
using ToDoList.Persistence;
using ToDoList.Persistence.Repositories;
using ToDoList.WebApi;


namespace ToDoList.Test.IntegrationTests;

public class PostTests : IAsyncLifetime
{
    private readonly ToDoItemsContext context;
    private readonly ToDoItemsController controller;
    private readonly ToDoItemsRepository repository;

    public PostTests()
    {
        context = new ToDoItemsContext("Data Source=../../../IntegrationTests/data/localdb_test.db");
        repository = new ToDoItemsRepository(context);
        controller = new ToDoItemsController(repository);
    }

    [Fact]
    public async Task Post_CreateItem_ReturnsCreatedResponse()
    {
        // Arrange
        var request = new ToDoItemCreateRequestDto("Utři prach", "utři prach z poliček", false);

        // Act
        var result = await controller.Create(request);
        var actionResult = result.Result;
        var getResult = await controller.Read();

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
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        try
        {
            context.ToDoItems.RemoveRange(context.ToDoItems);
            await context.SaveChangesAsync();
            //Reset ID počítače pro další testy
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
