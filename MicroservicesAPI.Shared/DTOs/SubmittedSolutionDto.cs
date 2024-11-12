using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class SubmittedSolutionDto
{
    [JsonPropertyName("UsersCode")]
    public string UsersCode { get; set; }
    
    [JsonPropertyName("MethodName")]
    public string MethodName { get; set; }
    
    [JsonPropertyName("TestingData")]
    public List<List<object>> TestingData { get; set; }
    
    [JsonPropertyName("TestingDataTypes")]
    public List<string> TestingDataTypes { get; set; }
    
    [JsonPropertyName("TimeLimitSeconds")]
    public int TimeLimitSeconds { get; set; }
    
    [JsonPropertyName("ExpectedResult")]
    public ExpectedResultDto ExpectedResult { get; set; }
}