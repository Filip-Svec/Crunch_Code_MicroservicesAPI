using Docker.DotNet;
using Docker.DotNet.Models;
using MicroservicesAPI.Shared.DTOs;
using MicroservicesAPI.Shared.Entities;
using MicroservicesAPI.Shared.Exceptions;
using Microsoft.Scripting.Hosting;


namespace MicroservicesAPI.Python.Services;

public class PythonService()
{
    public async Task<ResultResponseDto> ProcessUsersCode(
        SubmittedSolutionDto submittedSolutionDto, 
        TestingData testingData)
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
                testingData
            );
            
            // Run execution on a separate thread 
            var executeCodeTask = Task.Run(() =>
            {
                engine.Execute(driverCode, scope);
            });

            // Main thread waiting for either one to finish
            if (await Task.WhenAny(executeCodeTask, Task.Delay(5 * 1000)) == executeCodeTask)
            {
                await executeCodeTask; // Awaiting the Task to re-throw exceptions onto the main thread
            }
            else
            {
                throw new TimeoutException();
            }

            // Moves the internal pointer back to the start before reading
            outputStream.Seek(0, SeekOrigin.Begin);
            result = new StreamReader(outputStream).ReadToEnd().Trim();
        }
        catch (TimeoutException ex)
        {
            var response = new ResultResponseDto(GetExceptionTypeName(ex), ex.Message, "");
            
            _ = Task.Run(async () =>
            {
                try
                {
                    await RestartDockerContainerAsync();
                }
                catch (Exception restartEx)
                {
                    Console.WriteLine($"Failed to restart Docker container: {restartEx.GetType()}, {restartEx.Message}");
                }
            });
            return response;
        }
        catch (TypeMismatchException ex)
        {
            return new ResultResponseDto(GetExceptionTypeName(ex), ex.Message, "");
        }
        catch (ValueMismatchException ex)
        {
            outputStream.Seek(0, SeekOrigin.Begin);
            result = new StreamReader(outputStream).ReadToEnd().Trim();
            return new ResultResponseDto(GetExceptionTypeName(ex), ex.Message, result);
        }
        catch (Exception ex)
        {
            return new ResultResponseDto(GetExceptionTypeName(ex), ex.Message, "");
        }
        return new ResultResponseDto("Success", "this is fine", result);
    }
    
    private async Task RestartDockerContainerAsync()
    {
        // Reset docker by container label
        var labelKey = "service_name";
        var labelValue = "microservicesapi.python";

        using var dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();
        
        // list of containers
        var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true, // all containers (running, stopped, and paused)
            Filters = new Dictionary<string, IDictionary<string, bool>>
            {
                { "label", new Dictionary<string, bool> { { $"{labelKey}={labelValue}", true } } }
            }
        });     
        
        // from list to a single container
        var container = containers.FirstOrDefault();
        
        if (container != null)
        {
            await dockerClient.Containers.RestartContainerAsync(container.ID, new ContainerRestartParameters());
        }
        else
        {
            throw new Exception($"No container found with label '{labelKey}={labelValue}'.");
        }
    }
    
    private string GetExceptionTypeName(Exception ex)
    {
        string fullName = ex.GetType().ToString();
        int lastDotIndex = fullName.LastIndexOf('.');   // -1 if no dot
        // If no dot, return fullname; else, return substring after last dot
        return lastDotIndex == -1 ? fullName : fullName.Substring(lastDotIndex + 1);
    }
    
    private string FormatArgument(string type, string value)
    {
        return type switch
        {
            // list of strings --> ['str1', 'str2', ...]
            "int" or "float" or "bool" or "list" => value, 
            "str" => $"\"{value}\"", // Wrap strings in quotes
            _ => throw new NotSupportedException($"Unsupported argument type: {type}")
        };
    }
    
    private string DriverCodeGenerator(string usersCode, TestingData testingData) 
    {
        string datasets = string.Join(", ", testingData.TaskDatasets.Select(dataset =>
        {
            string formattedArguments = string.Join(", ", dataset.Input.Select(input =>
                FormatArgument(input.type, input.value)));

            string expectedResultType = dataset.Output.type switch
            {
                "int" => "int",
                "str" => "str",
                "float" => "float",
                "bool" => "bool",
                "list" => "list",
                _ => "object"
            };

            string expectedResultValue = FormatArgument(dataset.Output.type, dataset.Output.value);

            return $@"
            {{
                'arguments': [{formattedArguments}],
                'expectedResult_type': {expectedResultType},
                'expectedResult_value': {expectedResultValue}
            }}";
        }));
        
        string driverCode = $@"
from typing import List     # to be able to use type hints with lists --> List[str]
import clr
import time
clr.AddReference('MicroservicesAPI.Shared')  # Reference to C# assembly where exceptions are defined
from MicroservicesAPI.Shared.Exceptions import TypeMismatchException, ValueMismatchException

__name__ = '__main__'  # explicitly set name variable

{usersCode}  # user's Python code

if __name__ == '__main__':
    solution = Solution()  # Instantiate the solution class

    # List of test datasets
    test_cases = [
        {datasets}
    ]

    max_execution_time = 0  # Track the longest execution time
    
    # enumerate -> returns tuple (index of item, item)
    for index, test_case in enumerate(test_cases):
        arguments = test_case['arguments']
        expectedResult_type = test_case['expectedResult_type']
        expectedResult_value = test_case['expectedResult_value']

        # Start timer
        start_time = time.time()

        # Execute method, * unpack arguments
        result = solution.{testingData.ExecutionMethodName}(*arguments)

        # Stop timer, store longest exec time in ms
        elapsed_time = (time.time() - start_time) * 1000
        max_execution_time = max(max_execution_time, elapsed_time)

        # ToDo: raise exception if exec finishes but time limit is exceeded

        # Check result type
        if type(result) != expectedResult_type:
            print(f'Tests passed {{index}} / {{len(test_cases)}}')
            raise TypeMismatchException(f'Test Case {{index+1}} failed. Result type: {{type(result)}}, Expected type: {{expectedResult_type}}')

        # Check result value
        if result != expectedResult_value:
            print(f'Tests passed {{index}} / {{len(test_cases)}}')
            raise ValueMismatchException(f'Test Case {{index+1}} failed. Result value: {{result}}, Expected value: {{expectedResult_value}}')

    print(f'All tests succeeded: {{len(test_cases)}} / {{len(test_cases)}}; Maximum execution time: {{max_execution_time:.2f}} ms')
";
        return driverCode;
    }
}