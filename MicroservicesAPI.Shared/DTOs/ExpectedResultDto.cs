using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class ExpectedResultDto
{
    [JsonPropertyName("ValueType")] 
    public string ValueType { get; set; }

    [JsonPropertyName("ListType")] 
    public string ListType { get; set; }
    
    [JsonPropertyName("Value")]
    // CHANGE from string to OBJECT
    public object Value { get; set; }
    
}