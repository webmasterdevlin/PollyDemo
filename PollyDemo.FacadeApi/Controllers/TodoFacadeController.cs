using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using Polly.Registry;

namespace PollyDemo.FacadeApi.Controllers;

[ApiController]
[Route("api/facade/todos")]
public class TodoFacadeController(
    IHttpClientFactory clientFactory,
    ResiliencePipelineProvider<string> pipelineProvider)
    : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetTodoById(int id)
    {
        Guard.Against.OutOfRange(id, nameof(id), 1, 100, "Only 1 to 100 are accepted");
        
        var client = clientFactory.CreateClient("YourClient");

        try
        {
            var pipeline = pipelineProvider.GetPipeline("default");

            var url = $"http://localhost:5130/api/todos/{id}";
            
            var response = await pipeline.ExecuteAsync(
                async ct => await client.GetAsync(url, ct));

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