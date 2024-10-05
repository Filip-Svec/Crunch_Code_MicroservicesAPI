using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class SubmittedSolutionDto
{
    [JsonPropertyName("UsersCode")]
    public String UsersCode { get; set; }
    
    [JsonPropertyName("TimeLimitSeconds")]
    public int TimeLimitSeconds { get; set; }
    
    [JsonPropertyName("Result")]
    public ExpectedResultDto ExpectedResult { get; set; }
}