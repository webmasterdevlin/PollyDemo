using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;

namespace PollyDemo.FacadeApi.Controllers;

/// <summary>
/// API Controller responsible for handling requests related to "Todo" items through a facade pattern.
/// This controller interacts with a backend API and applies Polly policies such as retry and circuit breaker.
/// </summary>
[ApiController]
[Route("api/facade/todos")]
public class TodoFacadeController(IHttpClientFactory clientFactory) : ControllerBase
{
    /// <summary>
    /// Retrieves a "Todo" item by its ID, interacting with a backend API.
    /// This method checks the validity of the provided ID and uses Polly policies (retry and circuit breaker) 
    /// to handle potential transient failures when calling the backend API.
    /// </summary>
    /// <param name="id">The ID of the "Todo" item to retrieve. Must be between 1 and 100.</param>
    /// <returns>An <see cref="ActionResult"/> containing the "Todo" item if successful, or an error status if not.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetTodoById(int id)
    {
        Guard.Against.OutOfRange(id, nameof(id), 1, 100, "Only 1 to 100 are accepted");
        
        var client = clientFactory.CreateClient("YourClient");

        try
        {
            var response = await client.GetAsync($"http://localhost:5130/api/todos/{id}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> FacadeApi RECEIVED a SUCCESS");
                var data = await response.Content.ReadAsStreamAsync();
                return new OkObjectResult(data);
            }

            Console.WriteLine("--> FacadeApi RECEIVED a FAILURE");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (Exception e)
        {
            return StatusCode(503, "Circuit Breaker Opened");
        }
    }
}