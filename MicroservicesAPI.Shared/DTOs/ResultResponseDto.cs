using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class ResultResponseDto(String resultState, String debugMessage, String? result)
{
    [JsonPropertyName("ResultState")]
    public String ResultState { get; set; } = resultState;

    [JsonPropertyName("DebugMessage")]
    public String DebugMessage { get; set; } = debugMessage;
    
    [JsonPropertyName("Result")]
    public String? Result { get; set; } = result;
}