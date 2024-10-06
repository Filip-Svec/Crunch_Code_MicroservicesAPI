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
        var scope = engine.CreateScope(); // Isolating an execution of python code namespacewise

        var outputStream = new MemoryStream(); // Stream that stores data in memory
        var writer = new StreamWriter(outputStream, System.Text.Encoding.UTF8); // SW writes any output to the MS
        writer.AutoFlush = true; // Writer auto flushes its buffer after each write operation
        engine.Runtime.IO.SetOutput(outputStream, writer); // send any output to the MS via the SW instead of stdout

        string result;

        try
        {
            string driverCode = DriverCodeGenerator(
                submittedSolutionDto.UsersCode,
                submittedSolutionDto.MethodName,
                submittedSolutionDto.TestingData,
                submittedSolutionDto.ExpectedResult
            );
            Console.WriteLine(driverCode);
            // Run execution on a separate thread 
            var executeCodeTask = Task.Run(() => { engine.Execute(driverCode, scope); });

            // Main thread waiting for either one to finish
            if (await Task.WhenAny(executeCodeTask,
                    Task.Delay(submittedSolutionDto.TimeLimitSeconds * 1000)) == executeCodeTask)
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
        catch (Exception ex)
        {
            Console.WriteLine($"Execution error: {ex.Message}");
            Console.WriteLine($"Execution error: {ex.GetType()}");
            return new ResultResponseDto(ResultState.Other, ex.Message, "");
        }

        // TODO Compare type of result (or if null)

        // TODO compare result itself

        return new ResultResponseDto(ResultState.Success, "this is fine", result);
    }

    private string DriverCodeGenerator(string usersCode, string methodName, List<object> testDatasets, ExpectedResultDto expectedResult)
    {
        //string formattedArguments = string.Join(", ", testDatasets.Select(arg => $"\"{arg}\""));

        string driverCode = $@"
__name__ = '__main__'  # explicitly set name variable

{usersCode} # every method defined in a class needs to have the argument *self*

if __name__ == '__main__':
    solution = Solution()
    result = solution.{methodName}()

    print(result)

";

        //result = solution.{methodName}({formattedArguments})
        return driverCode;
    }
}