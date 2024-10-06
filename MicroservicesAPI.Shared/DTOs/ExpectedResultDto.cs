using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class ExpectedResultDto
{
    [JsonPropertyName("ValueType")]
    public string ValueType { get; set; }
    
    [JsonPropertyName("Value")]
    public string Value { get; set; }
    
}