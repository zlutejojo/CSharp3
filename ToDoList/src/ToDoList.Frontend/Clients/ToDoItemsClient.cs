using System;
using System.Net.Http.Json;
using ToDoList.Domain.DTOs;
using ToDoList.Frontend.Models;

namespace ToDoList.Frontend.Clients;

public class ToDoItemsClient : IToDoItemsClient
{
    private readonly HttpClient httpClient;
    // dependency injection
    public ToDoItemsClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    // tady pouzivam reprezentaci z Dashboardu
    public async Task<List<ToDoItemView>> ReadItemsAsync()
    {
        var toDoItemsViews = new List<ToDoItemView>();
        var response = await httpClient.GetFromJsonAsync<List<ToDoItemGetResponseDto>>("api/ToDoItems");

        toDoItemsViews = response.Select(dto => new ToDoItemView()
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            IsCompleted = dto.IsCompleted,
            Category = dto.Category
        }).ToList();

        return toDoItemsViews;
    }

    public async Task<ToDoItemView?> ReadItemByIdAsync(int itemId)
    {
        var response = await httpClient.GetFromJsonAsync<ToDoItemGetResponseDto>($"api/ToDoItems/{itemId}");

        var toDoItem = new ToDoItemView()
        {
            Id = response.Id,
            Name = response.Name,
            Description = response.Description,
            IsCompleted = response.IsCompleted,
            Category = response.Category
        };
        return toDoItem;
    }

    public async Task UpdateItemAsync(ToDoItemView item)
    {
        // try {}
        var itemRequest = new ToDoItemUpdateRequestDto(item.Name, item.Description, item.IsCompleted, item.Category);
        var response = await httpClient.PutAsJsonAsync($"api/ToDoItems/{item.Id}", itemRequest);
    }

    public Task DeleteItemAsync(int itemId)
    {
        return httpClient.DeleteAsync($"api/ToDoItems/{itemId}");
    }

    public async Task CreateItemAsync(ToDoItemView item) {
        var request = new ToDoItemCreateRequestDto(item.Name, item.Description, item.IsCompleted, item.Category);
        await httpClient.PostAsJsonAsync("api/ToDoItems", request);
    }
}
