using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;

namespace PollyDemo.Todo.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController: ControllerBase
{
    [HttpGet("{id:int}")]
    public ActionResult GetEventById(int id)
    {
        Guard.Against.OutOfRange(id, nameof(id), 1, 100, "Only 1 to 100 are accepted");
        
        var random = new Random();
        var randomNumber = random.Next(1, 101);
        if(randomNumber >= id )
        {
            Console.WriteLine("--> EventService Returned 500 ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        Console.WriteLine("--> EventService Returned 200 OK");
        return new OkObjectResult(new { id, activity = "Bruno Mars live in Oslo" });
    }
}
