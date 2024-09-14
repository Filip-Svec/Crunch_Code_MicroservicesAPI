using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class SubmittedCodeDto
{
    [JsonPropertyName("UsersCode")]
    public String UsersCode { get; set; }
    
    [JsonPropertyName("TimeLimitSeconds")]
    public int TimeLimitSeconds { get; set; }
    
    [JsonPropertyName("Result")]
    public SolutionDto Solution { get; set; }
}