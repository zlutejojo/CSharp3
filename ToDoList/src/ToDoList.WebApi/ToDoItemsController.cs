namespace ToDoList.WebApi;

using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Models;
using ToDoList.Domain.DTOs;
using ToDoList.Persistence;
using Humanizer;
using ToDoList.Persistence.Repositories;

[Route("api/[controller]")] //localhost:5000/api/ToDoItems
[ApiController]
public class ToDoItemsController : ControllerBase
{

    private static readonly List<ToDoItem> items = [];
    // private readonly ToDoItemsContext context;
    private readonly IRepository<ToDoItem> repository;

    // public ToDoItemsController(ToDoItemsContext context, IRepository<ToDoItem> repository)
    // {
    //     this.context = context;
    //     this.repository = repository;
    // }

    public ToDoItemsController(IRepository<ToDoItem> repository)
    {
        this.repository = repository;
    }

    [HttpPost]
    public IActionResult Create(ToDoItemCreateRequestDto request) //pouzijeme DTO - Data Transfer Object
    {
        ToDoItem item = request.ToDomain();

        //try to create an item
        try
        {
            repository.Create(item);
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
    public ActionResult<IEnumerable<ToDoItemGetResponseDto>> Read() //api/ToDoItems GET
    {
        try
        {
            var responseDtos = context.ToDoItems
                //převede každý ToDoItem z DB na ToDoItemGetResponseDto
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
    public IActionResult ReadById(int toDoItemId) //api/ToDoItems/<id> GET
    {
        try
        {
            ToDoItem item = context.ToDoItems.Find(toDoItemId);

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
    public IActionResult UpdateById(int toDoItemId, [FromBody]
    ToDoItemUpdateRequestDto request)
    {
        try
        {
            ToDoItem itemToUpdate = context.ToDoItems.Find(toDoItemId);

            if (itemToUpdate == null)
            {
                return NotFound(); // 404
            }

            // Update item properties
            itemToUpdate.Name = request.Name;
            itemToUpdate.Description = request.Description;
            itemToUpdate.IsCompleted = request.IsCompleted;

            context.SaveChanges();

            var responseDto = ToDoItemGetResponseDto.FromDomain(itemToUpdate);
            return Ok(responseDto); // 200
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError); // 500
        }
    }

    [HttpDelete("{toDoItemId:int}")]
    public IActionResult DeleteById(int toDoItemId)
    {
        try
        {
            ToDoItem itemToDelete = context.ToDoItems.Find(toDoItemId);

            if (itemToDelete == null)
            {
                return NotFound(); // 404
            }

            context.ToDoItems.Remove(itemToDelete);
            context.SaveChanges();

            return NoContent(); // 204
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError); // 500
        }
    }

    public void AddItemToStorage(ToDoItem item)
    {
        items.Add(item);
    }

    public void RemoveItemFromStorage(ToDoItem item)
    {
        items.Remove(item);
    }
}
