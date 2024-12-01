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
    [JsonPropertyName("PythonTemplate")]
    public string PythonTemplate { get; set; }
    [JsonPropertyName("PythonSolution")]
    public string PythonSolution { get; set; }
    [JsonPropertyName("PythonSolutionDesc")]
    public string PythonSolutionDesc { get; set; }
    [JsonPropertyName("JSTemplate")]
    public string JavaScriptTemplate { get; set; }
    [JsonPropertyName("JSSolution")]
    public string JavaScriptSolution { get; set; }
    [JsonPropertyName("JSSolutionDesc")]
    public string JavaScriptSolutionDesc { get; set; }
}