using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;

namespace PollyDemo.Todo.Controllers;

/// <summary>
/// API Controller responsible for handling requests related to "Todo" items.
/// This controller simulates responses with either a success or a failure based on a random number generation.
/// </summary>
[ApiController]
[Route("api/todos")]
public class TodoController: ControllerBase
{
    /// <summary>
    /// Retrieves a "Todo" item by its ID.
    /// This method checks the validity of the provided ID and simulates a success or failure response
    /// based on a random number comparison.
    /// </summary>
    /// <param name="id">The ID of the "Todo" item to retrieve. Must be between 1 and 100.</param>
    /// <returns>An <see cref="ActionResult"/> containing the "Todo" item if successful, or an error status if not.</returns>
    [HttpGet("{id:int}")]
    public ActionResult GetTodoById(int id)
    {
        Guard.Against.OutOfRange(id, nameof(id), 1, 100, "Only 1 to 100 are accepted");
        
        var random = new Random();
        var randomNumber = random.Next(1, 101);
        if(randomNumber >= id )
        {
            Console.WriteLine("--> TodoService Returned 500 ERROR");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        Console.WriteLine("--> TodoService Returned 200 OK");
        return new OkObjectResult(new { id, activity = "eat" });
    }
}
