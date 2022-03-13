using Microsoft.AspNetCore.Mvc;

namespace PollyDemo.Todo.Controllers;

[Route("api/todos")]
[ApiController]
public class TodoController: ControllerBase
{
    [HttpGet("{id:int}")]
    public ActionResult GetTodo(int id)
    {
        var random = new Random();
        var randomNumber = random.Next(1, 101);
        if(randomNumber >= id )
        {
            Console.WriteLine("-> Returning 500 ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        Console.WriteLine("-> Returning 200 OK");
        return Ok();
    }
}