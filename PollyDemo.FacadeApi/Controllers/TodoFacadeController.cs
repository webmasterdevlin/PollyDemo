using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using PollyDemo.FacadeApi.Policies;

namespace PollyDemo.FacadeApi.Controllers;

[ApiController]
[Route("api/facade/todos")]
public class TodoFacadeController : ControllerBase
{
    private readonly ClientPolicy _clientPolicy;
    private readonly IHttpClientFactory _clientFactory;

    public TodoFacadeController(ClientPolicy clientPolicy, IHttpClientFactory clientFactory)
    {
        _clientPolicy = clientPolicy;
        _clientFactory = clientFactory;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetTodoById(int id)
    {
        Guard.Against.OutOfRange(id, nameof(id), 1, 100, "Only 1 to 100 are accepted");

        var client = _clientFactory.CreateClient();

        var response = await _clientPolicy.ExponentialHttpRetry.ExecuteAsync(()
            => client.GetAsync($"https://localhost:7211/api/todos/{id}"));

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("--> FacadeApi RECEIVED a SUCCESS");
            return Ok();
        }

        Console.WriteLine("--> FacadeApi RECEIVED a FAILURE");
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}