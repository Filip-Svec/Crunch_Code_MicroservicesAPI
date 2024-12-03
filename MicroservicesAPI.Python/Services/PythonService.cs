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

            // TODO: START time counter
            // Run execution on a separate thread 
            var executeCodeTask = Task.Run(() =>
            {
                Console.WriteLine("start");
                engine.Execute(driverCode, scope);
                Console.WriteLine("end");
            });

            // Main thread waiting for either one to finish
            if (await Task.WhenAny(executeCodeTask, Task.Delay(7 * 1000)) == executeCodeTask)
            {
                // TODO: END time counter
                await executeCodeTask; // Awaiting the Task to re-throw exceptions onto the main thread
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
    
    # enumerate -> returns tuple (index of item, item)
    for index, test_case in enumerate(test_cases):
        arguments = test_case['arguments']
        expectedResult_type = test_case['expectedResult_type']
        expectedResult_value = test_case['expectedResult_value']

        # Execute method, * unpack arguments
        result = solution.{testingData.ExecutionMethodName}(*arguments)

        # Check result type
        if type(result) != expectedResult_type:
            print(f'Tests passed {{index}} / {{len(test_cases)}}')
            raise TypeMismatchException(f'Test Case {{index+1}} failed. Result type: {{type(result)}}, Expected type: {{expectedResult_type}}')

        # Check result value
        if result != expectedResult_value:
            print(f'Tests passed {{index}} / {{len(test_cases)}}')
            raise ValueMismatchException(f'Test Case {{index+1}} failed. Result value: {{result}}, Expected value: {{expectedResult_value}}')

    print(f'All tests succeeded: {{len(test_cases)}} / {{len(test_cases)}}')    
";
        return driverCode;
    }

    
    
// # If list, check type of first element
//     if isinstance(result, list):
//         if len(result) > 0:
//         if type(result[0]) != {expectedListType}:
//     raise TypeMismatchException(f'List element type: {{type(result[0])}}, Expected: {expectedListType}')

    
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