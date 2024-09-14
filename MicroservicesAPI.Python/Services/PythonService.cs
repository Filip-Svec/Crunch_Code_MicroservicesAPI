using System.Data;
using MicroservicesAPI.Shared;
using IronPython.Hosting;
using MicroservicesAPI.Shared.DTOs;
using Microsoft.Scripting.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MicroservicesAPI.Python.Services;

public class PythonService
{
    public async Task<ResultResponseDto> ProcessUsersCode(SubmittedCodeDto submittedCodeDto)
    {
        ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();

        try
        {
            await engine.Execute(submittedCodeDto.UsersCode);
        }
        catch (SyntaxErrorException ex)
        {
            Console.WriteLine($"Execution error: {ex.Message}");
            return new ResultResponseDto(ResultState.SyntaxError, ex.Message, "");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"Execution error: {ex.Message}");
            return new ResultResponseDto(ResultState.TimeLimitExceeded, ex.Message, "");
        }
        catch (OutOfMemoryException ex)
        {
            Console.WriteLine($"Execution error: {ex.Message}");
            return new ResultResponseDto(ResultState.OutOfMemory, ex.Message, "");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Execution error: {ex.Message}");
            return new ResultResponseDto(ResultState.Other, ex.Message, "");
        }
        
        
        // TODO Compare type of result (or if null)
        
        // TODO compare length of result
        
        // TODO compare result itself
        
        return new ResultResponseDto(ResultState.Success, "this is fine", "07");
    }

    
    // could replace the 'message' with a 'debug message' to more accurately specify the error
    
    // public ResultResponseDto BuildResponseDto(ResultState resultState)
    // {
    //     switch (resultState)
    //     {
    //         // code executed, result match
    //         case ResultState.Success:
    //             return new ResultResponseDto(resultState, "Message Success");
    //         
    //         // code executed, result doesn't match
    //         case ResultState.TypeMismatch:
    //             return new ResultResponseDto(resultState, "Message Not Success");
    //         
    //         // code executed, result doesn't match
    //         case ResultState.ValueMismatch:
    //             return new ResultResponseDto(resultState, "Message Not Success");
    //         
    //         // code not executed, no result
    //         case ResultState.SyntaxError:
    //             return new ResultResponseDto(resultState, "Message Not Success");
    //         
    //         // code not executed, no result
    //         case ResultState.OutOfMemory:
    //             return new ResultResponseDto(resultState, "Message Not Success");
    //         
    //         // code not executed, no result
    //         case ResultState.TimeLimitExceeded:
    //             return new ResultResponseDto(resultState, "Message Not Success");
    //         
    //         // code not executed, no result
    //         case ResultState.Unknown:
    //             return new ResultResponseDto(resultState, "Message Not Success");
    //             
    //         default:
    //             return new ResultResponseDto(resultState, "Message Other");
    //     }
    // }
    
}