using Microsoft.AspNetCore.Mvc;

namespace PollyDemo.Todo.Controllers;

[ApiController]
[Route("api/todos")]
public class TodoController: ControllerBase
{
    [HttpGet("{id:int}")]
    public ActionResult GetTodoById(int id)
    {
        var random = new Random();
        var randomNumber = random.Next(1, 101);
        if(randomNumber >= id )
        {
            Console.WriteLine("--> TodoService Returned 500 ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        Console.WriteLine("--> TodoService Returned 200 OK");
        return Ok();
    }
}