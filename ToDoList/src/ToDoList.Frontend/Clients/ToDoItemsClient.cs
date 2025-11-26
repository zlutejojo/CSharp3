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
        var toDoItemsView = new List<ToDoItemView>();
        var response = await httpClient.GetFromJsonAsync<List<ToDoItemGetResponseDto>>("api/ToDoItems");

        toDoItemsView = response.Select(dto => new ToDoItemView(
            dto.Id,
            dto.Name,
            dto.Description,
            dto.IsCompleted
            )).ToList();

        return toDoItemsView;
    }
}
