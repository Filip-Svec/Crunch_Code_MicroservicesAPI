using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class TaskRequestDto
{
    [JsonPropertyName("TaskName")]
    public string TaskName { get; set; }
    
    [JsonPropertyName("Language")]
    public string Language { get; set; }
}