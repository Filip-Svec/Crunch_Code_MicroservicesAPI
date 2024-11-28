using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class SubmittedSolutionDto
{
    [JsonPropertyName("UsersCode")]
    public string UsersCode { get; set; }
    
    [JsonPropertyName("TaskId")]
    public string TaskId { get; set; }
    
}