using System.Net;
using PollyDemo.FacadeApi.Models;

namespace PollyDemo.FacadeApi.Services;

public class TodoService
{
    private readonly HttpClient _httpClient;

    public TodoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<TodoResponse?> GetTodoById(int id)
    {
        var url = $"http://localhost:5130/api/todos/{id}";

        var response = await _httpClient.GetAsync(url);
        
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TodoResponse>();
    }
}