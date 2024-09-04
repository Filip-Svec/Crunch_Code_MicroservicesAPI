using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class SolutionDto
{
    [JsonPropertyName("ValueType")]
    public String ValueType { get; set; }
    
    [JsonPropertyName("Value")]
    public String Value { get; set; }
    
}