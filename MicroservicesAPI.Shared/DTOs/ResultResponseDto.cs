using System.Text.Json.Serialization;

namespace MicroservicesAPI.Shared.DTOs;

public class ResultResponseDto(ResultState resultState, string message)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonPropertyName("ErrorType")]
    public ResultState ResultState { get; set; } = resultState;

    [JsonPropertyName("Message")]
    public String Message { get; set; } = message;
}