using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;

namespace MicroservicesAPI.Common.DTOs;

public class SolutionDto
{
    [JsonPropertyName("ValueType")]
    public String ValueType { get; set; }
    
    [JsonPropertyName("Value")]
    public String Value { get; set; }
    
}