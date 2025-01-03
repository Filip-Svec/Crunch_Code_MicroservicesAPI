using System.Text.Json.Serialization;
using MicroservicesAPI.Shared.Entities.CodingTaskNested;

namespace MicroservicesAPI.Shared.DTOs;

public class TaskResponseDto
{
    [JsonPropertyName("Id")]
    public string Id { get; set; }
    
    [JsonPropertyName("TaskName")]
    public string TaskName { get; set; }
    
    [JsonPropertyName("Description")]
    public string Description { get; set; }
    
    [JsonPropertyName("Examples")]
    public List<Examples> Examples { get; set; }
    
    [JsonPropertyName("Constraints")]
    public List<string> Constraints { get; set; }
    
    [JsonPropertyName("Hints")]
    public List<string> Hints { get; set; }
    
    [JsonPropertyName("Template")]
    public string Template { get; set; }
    
    [JsonPropertyName("Solution")]
    public string Solution { get; set; }
    
    [JsonPropertyName("SolutionDesc")]
    public string SolutionDesc { get; set; }
}