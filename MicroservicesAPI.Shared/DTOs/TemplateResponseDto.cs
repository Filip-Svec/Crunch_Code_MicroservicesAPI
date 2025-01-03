using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class TemplateResponseDto
{
    [JsonPropertyName("Template")]
    public string Template { get; set; }
    
    [JsonPropertyName("Solution")]
    public string Solution { get; set; }
    
    [JsonPropertyName("SolutionDesc")]
    public string SolutionDesc { get; set; }
}