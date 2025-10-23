namespace ToDoList.WebApi;

using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Models;
using ToDoList.Domain.DTOs;
using ToDoList.Persistence;
using Humanizer;

[Route("api/[controller]")] //localhost:5000/api/ToDoItems
[ApiController]
public class ToDoItemsController : ControllerBase
{

    private static readonly List<ToDoItem> items = [];
    private readonly ToDoItemsContext context;

    public ToDoItemsController(ToDoItemsContext context)
    {
        this.context = context;
    }

    [HttpPost]
    public IActionResult Create(ToDoItemCreateRequestDto request) //pouzijeme DTO - Data Transfer Object
    {
        ToDoItem item = request.ToDomain();

        //try to create an item
        try
        {
            // item.ToDoItemId = items.Count == 0 ? 1 : items.Max(o => o.ToDoItemId) + 1;
            // items.Add(item);
            context.ToDoItems.Add(item);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError); //500
        }

        //respond to client
        ToDoItemGetResponseDto responseDto = ToDoItemGetResponseDto.FromDomain(item);
        return Created();
    }

    [HttpGet]
    public ActionResult<IEnumerable<ToDoItemGetResponseDto>> Read() //api/ToDoItems GET
    {
        try
        {
            if (items == null)
            {
                return NotFound(); // 404
            }

            if (items.Count == 0)
            {
                return NotFound(); // 404
            }

            List<ToDoItemGetResponseDto> responseDtos = new List<ToDoItemGetResponseDto>();
            foreach (ToDoItem item in items)
            {
                ToDoItemGetResponseDto dto = ToDoItemGetResponseDto.FromDomain(item);
                responseDtos.Add(dto);
            }
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
            ToDoItem item = items.Find(i => i.ToDoItemId == toDoItemId);

            if (item == null)
            {
                return NotFound(); // 404
            }

            if (items.Count == 0)
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
            ToDoItem itemToUpdate = items.Find(i => i.ToDoItemId == toDoItemId);

            if (itemToUpdate == null)
            {
                return NotFound(); // 404
            }

            if (items.Count == 0)
            {
                return NotFound(); // 404
            }

            // Update item properties
            itemToUpdate.Name = request.Name;
            itemToUpdate.Description = request.Description;
            itemToUpdate.IsCompleted = request.IsCompleted;

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
            ToDoItem itemToDelete = items.Find(i => i.ToDoItemId == toDoItemId);

            if (itemToDelete == null)
            {
                return NotFound(); // 404
            }

            if (items.Count == 0)
            {
                return NotFound(); // 404
            }

            items.Remove(itemToDelete);

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
