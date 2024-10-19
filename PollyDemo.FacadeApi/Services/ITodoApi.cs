using PollyDemo.FacadeApi.Models;
using Refit;

namespace PollyDemo.FacadeApi.Services;

public interface ITodoApi
{
    [Get("/api/todos/{id}")]
    Task<TodoItem> GetTodoByIdAsync(int id);
}
