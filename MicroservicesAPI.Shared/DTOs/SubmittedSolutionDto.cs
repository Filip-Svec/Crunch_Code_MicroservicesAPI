using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class SubmittedSolutionDto
{
    [JsonPropertyName("UsersCode")]
    public String UsersCode { get; set; }
    
    [JsonPropertyName("MethodName")]
    public string MethodName { get; set; }
    
    [JsonPropertyName("TestingData")]
    public List<object> TestingData { get; set; } 
    
    [JsonPropertyName("TimeLimitSeconds")]
    public int TimeLimitSeconds { get; set; }
    
    [JsonPropertyName("ExpectedResult")]
    public ExpectedResultDto ExpectedResult { get; set; }
}