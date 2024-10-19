using System.Text.Json.Serialization;

namespace PollyDemo.FacadeApi.Models;

public class TodoResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("activity")]
    public string Activity { get; set; }
    
    [JsonPropertyName("completed")]
    public bool Completed { get; set; }
}