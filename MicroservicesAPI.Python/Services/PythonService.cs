using MicroservicesAPI.Common.DTOs;
using MicroservicesAPI.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MicroservicesAPI.Python.Services;

public class PythonService
{
    public async Task<ResultState> ProcessUsersCode(SubmittedCodeDto submittedCodeDto)
    {





        return ResultState.Success;
    }

    public ResultResponseDto BuildResponseDto(ResultState resultState)
    {
        switch (resultState)
        {
            case ResultState.Success:
                
                return new ResultResponseDto(resultState, "Message Success");
                
            case ResultState.TypeMismatch:
                return new ResultResponseDto(resultState, "Message Not Success");
            
            case ResultState.ValueMismatch:
                return new ResultResponseDto(resultState, "Message Not Success");
            
            case ResultState.SyntaxError:
                return new ResultResponseDto(resultState, "Message Not Success");
            
            case ResultState.OutOfMemory:
                return new ResultResponseDto(resultState, "Message Not Success");
            
            case ResultState.TimeLimitExceeded:
                return new ResultResponseDto(resultState, "Message Not Success");
            
            case ResultState.Unknown:
                return new ResultResponseDto(resultState, "Message Not Success");
                
            default:
                return new ResultResponseDto(resultState, "Message Other");
        }
    }
    
}