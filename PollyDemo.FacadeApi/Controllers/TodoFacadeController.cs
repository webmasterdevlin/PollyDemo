using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;

namespace PollyDemo.FacadeApi.Controllers;

[ApiController]
[Route("api/facade/todos")]
public class TodoFacadeController(IHttpClientFactory clientFactory) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetTodoById(int id)
    {
        Guard.Against.OutOfRange(id, nameof(id), 1, 100, "Only 1 to 100 are accepted");
        
        var client = clientFactory.CreateClient("YourClient");

        try
        {
            var response = await client.GetAsync($"https://localhost:7211/api/todos/{id}");

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