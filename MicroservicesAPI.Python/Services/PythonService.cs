using System.Data;
using System.Runtime.InteropServices;
using MicroservicesAPI.Shared;
using IronPython.Hosting;
using IronPython.Modules;
using MicroservicesAPI.Shared.DTOs;
using Microsoft.Scripting.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MicroservicesAPI.Python.Services;

public class PythonService
{
    public async Task<ResultResponseDto> ProcessUsersCode(SubmittedSolutionDto submittedSolutionDto)
    {
        ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();

        try
        {
            // Run execution on a separate thread 
            var executeCodeTask = Task.Run(() => 
            {
                engine.Execute(submittedSolutionDto.UsersCode);
            }); 

            // Main thread waiting for either one to finish
            if (await Task.WhenAny(executeCodeTask, 
                    Task.Delay(submittedSolutionDto.TimeLimitSeconds*1000)) == executeCodeTask)
            {
                // Awaiting the Task to re-throw exceptions from within the Task
                // The task is not run again
                await executeCodeTask;
            }
            else 
            { 
                // TODO exit the process after it exceeded allotted time (Cancellation Token)
                throw new TimeoutException();
            }
        }
        catch (Microsoft.Scripting.SyntaxErrorException ex)
        {
            Console.WriteLine($"Execution error: {ex.Message}");
            return new ResultResponseDto(ResultState.SyntaxError, ex.Message, "");
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"Execution error: {ex.Message}");
            return new ResultResponseDto(ResultState.DivideByZero, ex.Message, "");
        }
        catch (IronPython.Runtime.UnboundNameException ex)
        {
            Console.WriteLine($"Execution error: {ex.Message}");
            return new ResultResponseDto(ResultState.UnboundName, ex.Message, "");
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
            Console.WriteLine($"Execution error: {ex.GetType()}");
            return new ResultResponseDto(ResultState.Other, ex.Message, "");
        }
        
        
        // TODO Compare type of result (or if null)
        
        // TODO compare length of result
        
        // TODO compare result itself
        
        return new ResultResponseDto(ResultState.Success, "this is fine", "07");
    }
    
}