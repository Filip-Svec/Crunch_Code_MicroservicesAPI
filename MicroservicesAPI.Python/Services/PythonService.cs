using System.Diagnostics;
using System.Runtime.Serialization;
using MicroservicesAPI.Shared.DTOs;
using MicroservicesAPI.Shared.Entities;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MicroservicesAPI.Python.Services;

public class PythonService()
{
    public async Task<ResultResponseDto> ProcessUsersCode(
        SubmittedSolutionDto submittedSolutionDto, 
        TestingData testingData)
    {
        string result = "";
        string tempFile = Path.GetTempFileName() + ".py";

        try
        {
            string driverCode = DriverCodeGenerator(
                submittedSolutionDto.UsersCode,
                testingData
            );
            
            await File.WriteAllTextAsync(tempFile, driverCode);

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "python3";
                process.StartInfo.Arguments = tempFile;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                var exitTask = process.WaitForExitAsync();

                if (await Task.WhenAny(exitTask, Task.Delay(5000)) == exitTask)
                {
                    result = await process.StandardOutput.ReadToEndAsync();
                }
                else
                {
                    process.Kill();
                    throw new TimeoutException();
                }
            }
            
            try
            {
                return JsonSerializer.Deserialize<ResultResponseDto>(result)
                       ?? throw new JsonSerializationException("Error in deserialization of python result response.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("deserialization");
            }
        }
        catch (TimeoutException ex)
        {
            return new ResultResponseDto(GetExceptionTypeName(ex), ex.Message, "");
        }
        catch (Exception ex)
        {
            return new ResultResponseDto(GetExceptionTypeName(ex), ex.Message, "");
        }
        finally
        {
            File.Delete(tempFile);
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
import time
import json
import sys
import traceback


user_code = """"""{usersCode}""""""

def main():
    try:
        # Compile the user's code to check for syntax errors
        compiled_code = compile(user_code, ""<string>"", ""exec"")
    except (SyntaxError, IndentationError) as e:
        print(json.dumps({{
            'ResultState': type(e).__name__,
            'DebugMessage': f'{{e.msg}} at line {{e.lineno}}',
            'Result': ''
        }}, ensure_ascii=False, indent=4), flush=True)
        sys.exit(1)  # Exit early since the script is invalid

    namespace = {{}}
    try:
        exec(compiled_code, namespace)

        solution_class = namespace.get(""Solution"")  
        if not solution_class:
            raise NameError(""Class 'Solution' is not defined in user code"")

        solution = solution_class()  # Instantiate the solution class

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
            result = getattr(solution, ""{testingData.ExecutionMethodName}"")(*arguments)

            # Stop timer, store longest exec time in ms
            elapsed_time = (time.time() - start_time) * 1000
            max_execution_time = max(max_execution_time, elapsed_time)

            # Check result type
            if type(result) != expectedResult_type:
                raise TypeError(f'Test Case {{index+1}} failed. Result type: {{type(result)}}, Expected type: {{expectedResult_type}}')

            # Check result value
            if result != expectedResult_value:
                raise ValueError(f'Test Case {{index+1}} failed. Result value: {{result}}, Expected value: {{expectedResult_value}}')

        print(json.dumps({{
            'ResultState': 'Success',
            'DebugMessage': 'All tests passed',
            'Result': 'Max execution time: {{:.2f}} ms'.format(max_execution_time)
        }}))

    except Exception as e:
        print(json.dumps({{
            'ResultState': type(e).__name__,
            'DebugMessage': str(e),
            'Result': ''
        }}, ensure_ascii=False, indent=4), flush=True)

    except SystemExit:
        print(json.dumps({{
            'ResultState': 'SystemExit',
            'DebugMessage': 'Process attempted to exit',
            'Result': ''
        }}, ensure_ascii=False, indent=4), flush=True)

    except BaseException as e:  # Catches everything else (like MemoryError)
        print(json.dumps({{
            'ResultState': 'CriticalError',
            'DebugMessage': str(e),
            'Result': ''
        }}, ensure_ascii=False, indent=4), flush=True)

if __name__ == '__main__':
    main()
    ";
        return driverCode;
    }
}