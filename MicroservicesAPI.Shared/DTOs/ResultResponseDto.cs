using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class ResultResponseDto(ResultState resultState, String debugMessage, String? result)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonPropertyName("ErrorType")]
    public ResultState ResultState { get; set; } = resultState;

    [JsonPropertyName("DebugMessage")]
    public String DebugMessage { get; set; } = debugMessage;
    
    [JsonPropertyName("Result")]
    public String? Result { get; set; } = result;
}