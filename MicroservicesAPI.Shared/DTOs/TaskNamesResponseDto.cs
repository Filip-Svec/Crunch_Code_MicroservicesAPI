using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class TaskNamesResponseDto
{
    [JsonPropertyName("AllTaskNames")]
    public List<string> AllTaskNames { get; set; }
}