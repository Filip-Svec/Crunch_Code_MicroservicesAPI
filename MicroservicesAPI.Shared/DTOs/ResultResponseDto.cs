using System.Text.Json.Serialization;
using MicroservicesAPI.Shared;

namespace MicroservicesAPI.Common.DTOs;

public class ResultResponseDto
{
    public ResultResponseDto(ResultState resultState, string message)
    {
        ResultState = resultState;
        Message = message;
    }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonPropertyName("ErrorType")]
    public ResultState ResultState { get; set; }
    
    [JsonPropertyName("Message")]
    public String Message { get; set; }
}