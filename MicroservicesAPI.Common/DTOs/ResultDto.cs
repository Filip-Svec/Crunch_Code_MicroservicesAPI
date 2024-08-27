using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;

namespace MicroservicesAPI.Common.DTOs;

public class ResultDto
{
    [JsonPropertyName("Type")]
    public String Type { get; set; }
    
    [JsonPropertyName("Value")]
    public String Value { get; set; }
}