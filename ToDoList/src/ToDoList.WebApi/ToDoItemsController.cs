namespace ToDoList.WebApi;

using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Models;
using ToDoList.Domain.DTOs;

[Route("api/[controller]")] //localhost:5000/api/ToDoItems
[ApiController]
public class ToDoItemsController : ControllerBase
{

    private static readonly List<ToDoItem> items = [];

    [HttpPost]
    public IActionResult Create(ToDoItemCreateRequestDto request) //pouzijeme DTO - Data Transfer Object
    {
        ToDoItem item = request.ToDomain();

        //try to create an item
        try
        {
            item.ToDoItemId = items.Count == 0 ? 1 : items.Max(o => o.ToDoItemId) + 1;
            items.Add(item);
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
    public IActionResult Read() //api/ToDoItems GET
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

            var responseDtos = new List<ToDoItemGetResponseDto>();
            foreach (var item in items)
            {
                var dto = ToDoItemGetResponseDto.FromDomain(item);
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
        return Ok(); //200
    }

    [HttpDelete("{toDoItemId:int}")]
    public IActionResult DeleteById()
    {
        return Ok(); //200
    }
}
