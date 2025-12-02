using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.Persistence;
using ToDoList.Persistence.Repositories;
using ToDoList.WebApi;


namespace ToDoList.Test.IntegrationTests;

public class PutTests : IAsyncLifetime
{
    private readonly ToDoItemsContext context;
    private readonly ToDoItemsController controller;
    private readonly ToDoItemsRepository repository;

    public PutTests()
    {
        context = new ToDoItemsContext("Data Source=../../../IntegrationTests/data/localdb_test.db");
        repository = new ToDoItemsRepository(context);
        controller = new ToDoItemsController(repository);
    }

    [Fact]
    public async Task Put_UpdateItem_ReturnsCreatedResponse()
    {
        // Arrange
        var originalItem = new ToDoItem { Name = "Vyper", Description = "Vyper barevné prádlo" };
        context.ToDoItems.Add(originalItem);
        await context.SaveChangesAsync();

        var request = new ToDoItemUpdateRequestDto("Vyper", "Vyper bílé prádlo", true);

        // Act
        var actionResult = await controller.UpdateById(originalItem.ToDoItemId, request);

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
    public async Task Put_UpdateNonExistentItem_ReturnsNotFound()
    {
        // Arrange
        var request = new ToDoItemUpdateRequestDto("Nic", "Nic", false);
        //předpokládám, že v seznamu není žádná položka s tímto ID
        int nonExistentId = 99999;

        // Act
        // Zavoláme metodu pro update s neexistujícím ID.
        var actionResult = await controller.UpdateById(nonExistentId, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        context.ToDoItems.RemoveRange(context.ToDoItems);
        await context.SaveChangesAsync();
        await context.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name='ToDoItems'");
        await context.DisposeAsync();
    }
}
