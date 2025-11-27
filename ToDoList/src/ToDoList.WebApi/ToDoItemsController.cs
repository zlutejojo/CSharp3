namespace ToDoList.WebApi;

using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Models;
using ToDoList.Domain.DTOs;
using ToDoList.Persistence.Repositories;
using ToDoList.Persistence;

[Route("api/[controller]")] //localhost:5000/api/ToDoItems
[ApiController]
public class ToDoItemsController : ControllerBase
{

    private readonly IRepositoryAsync<ToDoItem> repository;

    public ToDoItemsController(IRepositoryAsync<ToDoItem> repository)
    {
        this.repository = repository;
    }

    [HttpPost]
    public async Task<ActionResult<ToDoItemGetResponseDto>> Create(ToDoItemCreateRequestDto request) //pouzijeme DTO - Data Transfer Object
    {
        ToDoItem item = request.ToDomain();

        //try to create an item
        try
        {
            await repository.CreateAsync(item);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError); //500
        }

        //respond to client
        ToDoItemGetResponseDto responseDto = ToDoItemGetResponseDto.FromDomain(item);
        return CreatedAtAction(
            nameof(ReadById),
            new { toDoItemId = item.ToDoItemId },
            responseDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToDoItemGetResponseDto>>> Read() //api/ToDoItems GET
    {
        try
        {
            var itemsFromDb = await repository.GetAllAsync();
            var responseDtos = itemsFromDb
                .Select(item => ToDoItemGetResponseDto.FromDomain(item))
                .ToList();
            return Ok(responseDtos); // 200
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError); // 500
        }
    }

    [HttpGet("{toDoItemId:int}")]
    public async Task<ActionResult<ToDoItemGetResponseDto>> ReadById(int toDoItemId) //api/ToDoItems/<id> GET
    {
        try
        {
            ToDoItem item = await repository.GetByIdAsync(toDoItemId);
            if (item == null)
            {
                return NotFound(); // 404
            }

            ToDoItemGetResponseDto responseDto = ToDoItemGetResponseDto.FromDomain(item);
            return Ok(responseDto); // 200
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError); //500
        }
    }

    [HttpPut("{toDoItemId:int}")]
    public async Task<IActionResult> UpdateById(int toDoItemId, [FromBody]
    ToDoItemUpdateRequestDto request)
    {
        try
        {
            ToDoItem itemToUpdate = await repository.GetByIdAsync(toDoItemId);

            if (itemToUpdate == null)
            {
                return NotFound(); // 404
            }

            // Update item properties
            itemToUpdate.Name = request.Name;
            itemToUpdate.Description = request.Description;
            itemToUpdate.IsCompleted = request.IsCompleted;
            itemToUpdate.Category = request.Category;

            await repository.UpdateAsync(itemToUpdate);

            var responseDto = ToDoItemGetResponseDto.FromDomain(itemToUpdate);
            return Ok(responseDto); // 200
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError); // 500
        }
    }

    [HttpDelete("{toDoItemId:int}")]
    public async Task<IActionResult> DeleteById(int toDoItemId)
    {
        try
        {
            ToDoItem itemToDelete = await repository.GetByIdAsync(toDoItemId);
            if (itemToDelete == null)
            {
                return NotFound(); // 404
            }

            await repository.DeleteAsync(toDoItemId);

            return NoContent(); // 204
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError); // 500
        }
    }
}
