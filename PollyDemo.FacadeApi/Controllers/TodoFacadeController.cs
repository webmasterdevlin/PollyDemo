using Microsoft.AspNetCore.Mvc;
using PollyDemo.FacadeApi.Policies;

namespace PollyDemo.FacadeApi.Controllers;

[Route("api/facade/todos")]
[ApiController]
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
    public async Task<ActionResult> GoTodo(int id)
    {
        var client = _clientFactory.CreateClient();
        
        var response = await _clientPolicy.ExponentialHttpRetry.ExecuteAsync(()
            => client.GetAsync($"https://localhost:7211/api/todos/{id}"));

        if(response.IsSuccessStatusCode)
        {
            Console.WriteLine("--> TodoService returned a SUCCESS");
            return Ok();
        }

        Console.WriteLine("--> TodoService returned a FAILURE");
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}