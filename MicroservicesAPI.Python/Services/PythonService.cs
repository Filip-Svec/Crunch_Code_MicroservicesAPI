using System.Data;
using System.Runtime.InteropServices;
using MicroservicesAPI.Shared;
using IronPython.Hosting;
using IronPython.Modules;
using MicroservicesAPI.Shared.DTOs;
using MicroservicesAPI.Shared.Exceptions;
using Microsoft.Scripting.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;


namespace MicroservicesAPI.Python.Services;

public class PythonService
{
    public async Task<ResultResponseDto> ProcessUsersCode(SubmittedSolutionDto submittedSolutionDto)
    {
        ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();
        var scope = engine.CreateScope(); // Isolating an execution of python code namespacewise

        var outputStream = new MemoryStream(); // Stream that stores data in memory
        var writer = new StreamWriter(outputStream, System.Text.Encoding.UTF8); // SW writes any output to the MS
        writer.AutoFlush = true; // Writer auto flushes its buffer after each write operation
        engine.Runtime.IO.SetOutput(outputStream, writer); // send any output to the MS via the SW instead of stdout

        string result = "";

        try
        {
            string driverCode = DriverCodeGenerator(
                submittedSolutionDto.UsersCode,
                submittedSolutionDto.MethodName,
                submittedSolutionDto.TestingData,
                submittedSolutionDto.TestingDataTypes,
                submittedSolutionDto.ExpectedResult
            );

            // Run execution on a separate thread 
            var executeCodeTask = Task.Run(() => { engine.Execute(driverCode, scope); });

            // Main thread waiting for either one to finish
            if (await Task.WhenAny(executeCodeTask, Task.Delay(submittedSolutionDto.TimeLimitSeconds * 1000)) == executeCodeTask)
            {
                await executeCodeTask; // Awaiting the Task to re-throw exceptions from within
            }
            else
            {
                // TODO exit the process after it exceeded allotted time (Cancellation Token)
                throw new TimeoutException();
            }

            // Moves the internal pointer back to the start before reading
            outputStream.Seek(0, SeekOrigin.Begin);
            result = new StreamReader(outputStream).ReadToEnd().Trim();
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
        catch (Microsoft.Scripting.ArgumentTypeException ex)
        {
            Console.WriteLine($"Execution error: {ex.Message}");
            return new ResultResponseDto(ResultState.ArgumentType, ex.Message, "");
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
        catch (TypeMismatchException ex)
        {
            return new ResultResponseDto(ResultState.TypeMismatch, ex.Message, "");
        }
        catch (ValueMismatchException ex)
        {
            outputStream.Seek(0, SeekOrigin.Begin);
            result = new StreamReader(outputStream).ReadToEnd().Trim();
            return new ResultResponseDto(ResultState.ValueMismatch, ex.Message, result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Execution error: {ex.Message}");
            Console.WriteLine($"Execution error: {ex.GetType()}");
            
            return new ResultResponseDto(ResultState.Other, ex.Message, "");
        }

        return new ResultResponseDto(ResultState.Success, "this is fine", result);
    }
    // serilog, seq
    private string DriverCodeGenerator(string usersCode, string methodName, List<List<object>> testDatasets, List<string> testingDataTypes, ExpectedResultDto expectedResult)
    {
        string expectedType = expectedResult.ValueType switch
        {
            "int" => "int", "str" => "str", "float" => "float", "bool" => "bool", "list" => "list",
            _ => "object"       // default
        };
        string expectedListType = expectedResult.ListType switch
        {
            "int" => "int", "str" => "str", "float" => "float", "bool" => "bool", "list" => "list",
            _ => "object"       // default
        };
        
        // Format arguments 
        string formattedArguments = string.Join(", ", testDatasets[0].Zip(testingDataTypes, (arg, type) =>
        {
            return type switch
            {
                "int" or "float" or "bool" => arg.ToString(),    
                "str" => $"\"{arg}\"",                          // Strings wrap in quotes (or send with '' -> "'string'")
                // TODO lists
                _ => throw new NotSupportedException($"Unsupported argument type: {type}")
            };
        }));
        
        string driverCode = $@"
import clr
clr.AddReference('MicroservicesAPI.Shared')  # Reference to C# assembly where exceptions are defined
from MicroservicesAPI.Shared.Exceptions import TypeMismatchException, ValueMismatchException 

__name__ = '__main__'  # explicitly set name variable

{usersCode} # every method defined in a class needs to have the argument *self*

if __name__ == '__main__':
    solution = Solution()

    # for-cycle

    result = solution.{methodName}({formattedArguments})

    # Check result type
    if type(result) != {expectedType}:
         raise TypeMismatchException(f'Result: {{type(result)}}, Expected: {expectedType}')

    # If list, check type of first element
    if isinstance(result, list):
        if len(result) > 0:
            if type(result[0]) != {expectedListType}:
                raise TypeMismatchException(f'List element type: {{type(result[0])}}, Expected: {expectedListType}')
        
    # Check result value
    if result != {expectedResult.Value}:
        print(result, flush=True)
        raise ValueMismatchException(""Result does not match the Expected result"")

    print(result)
";
        
        return driverCode;
    }
    
    
    
    
    // private string FormatList(List<object> list)
    // {
    //     // This method will recursively format lists. You can extend it as needed.
    //     return "[" + string.Join(", ", list.Select(item =>
    //     {
    //         return item switch
    //         {
    //             int or float or bool => item.ToString(),
    //             string str => $"\"{str}\"",         // Wrap strings in quotes
    //             List<object> nestedList => FormatList(nestedList),  // Recursive call for nested lists
    //             _ => throw new NotSupportedException("Unsupported list element type")
    //         };
    //     })) + "]";
    // }
}