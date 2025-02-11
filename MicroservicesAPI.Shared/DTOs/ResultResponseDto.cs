using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class ResultResponseDto(String resultState, String debugMessage, String? resultStatusCode)
{
    [JsonPropertyName("ResultState")]
    public String ResultState { get; set; } = resultState;

    [JsonPropertyName("DebugMessage")]
    public String DebugMessage { get; set; } = debugMessage;
    
    [JsonPropertyName("ResultStatusCode")]
    public String? ResultStatusCode { get; set; } = resultStatusCode;
}