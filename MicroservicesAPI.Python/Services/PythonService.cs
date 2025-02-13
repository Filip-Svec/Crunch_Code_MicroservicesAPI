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
        string result = "";                                 // Stores output from python driver code
        string tempFile = Path.GetTempFileName() + ".py";   // Temporary file, stores python driver code

        try
        {
            string driverCode = DriverCodeGenerator(        // Generates python driver code that will be executed
                submittedSolutionDto.UsersCode,
                testingData
            );
            
            // Writes (async) generated Python code to the temporary file
            await File.WriteAllTextAsync(tempFile, driverCode);

            // Starts a new process to execute the Python script (driver code)
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "python3";             // Executable to run
                process.StartInfo.Arguments = tempFile;             // Pass the temporary file (driver code)
                process.StartInfo.RedirectStandardOutput = true;    // Capture stdout
                process.StartInfo.RedirectStandardError = true;     // Capture stderr
                process.StartInfo.UseShellExecute = false;          // False - allows redirection of stdout & err
                process.StartInfo.CreateNoWindow = true;            // Runs in the background (no window required)
                process.Start();                                    // Start process (execute script in temp file)

                var exitTask = process.WaitForExitAsync();          // Waits for process to finish

                // Time limit defined by Task delay, Which ever task finishes first
                if (await Task.WhenAny(exitTask, Task.Delay(5000)) == exitTask)
                {
                    result = await process.StandardOutput.ReadToEndAsync();     // Read process output
                }
                else
                {
                    process.Kill();
                    throw new TimeoutException();
                }
            }
            
            // Parse result into ResultResponseDto
            return JsonSerializer.Deserialize<ResultResponseDto>(result)
                   ?? throw new JsonSerializationException("Error in deserialization of python result response.");
            
        }
        catch (TimeoutException ex)     // Process (execution of driver code) ran for too long
        {
            return new ResultResponseDto(GetExceptionTypeName(ex), ex.Message, "422");
        }
        catch (Exception ex)            // All other non-user related errors
        {
            return new ResultResponseDto(GetExceptionTypeName(ex), ex.Message, "500");
        }
        finally
        {
            File.Delete(tempFile);      // Ensure temporary file is deleted after 
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
        // int, float, bool, list - as is; strings - in ""; else - throw
        return type switch
        {
            "int" or "float" or "bool" or "list" => value,      // List of strings --> ['str1', 'str2', ...]
            "str" => $"\"{value}\"",
            _ => throw new NotSupportedException($"Unsupported argument type in DB: {type}")
        };
    }
    
    private string DriverCodeGenerator(string usersCode, TestingData testingData) 
    {
        // * Depends on DB string storage --> useful if - string: "word", useless if - string: "'word'"
        // Join each dataset - "dataset1, dataset2,..."
        string datasets = string.Join(", ", testingData.TaskDatasets.Select(dataset =>
        {
            // Join formatted arguments - "arg1, arg2, ..." 
            string formattedArguments = string.Join(", ", dataset.Input.Select(input =>
                // *
                FormatArgument(input.type, input.value))
            );
            
            // *, only if there is an error in DB, this should never be needed
            string expectedResultType = dataset.Output.type switch
            {
                "int" => "int", "str" => "str", "float" => "float", "bool" => "bool", "list" => "list",
                _ => "object"
            };
            
            // *
            string expectedResultValue = FormatArgument(dataset.Output.type, dataset.Output.value);

            // Return dataset
            return $@"
            {{
                'arguments': [{formattedArguments}],
                'expectedResult_type': {expectedResultType},
                'expectedResult_value': {expectedResultValue}
            }}";
        }));
        
        string driverCode = $@"
import time
import json

class ResultTypeMismatchError(Exception):
    """"""Raised when the result type does not match the expected type.""""""
    pass

class ResultValueMismatchError(Exception):
    """"""Raised when the result value does not match the expected value.""""""
    pass

def printResponse(resultState: str, debugMessage: str, resultStatusCode: str):
    print(json.dumps({{
        'ResultState': resultState,
        'DebugMessage': debugMessage,
        'ResultStatusCode': resultStatusCode
    }}))

def main():

    user_code = """"""{usersCode}""""""                         # Get user's code
    execution_method = ""{testingData.ExecutionMethodName}""    # Get execution method name
    test_cases = [{datasets}]                                   # Get list of test datasets
    max_execution_time = 0                                      # Track the longest execution time
    user_code_namespace = {{}}                                  # Create sandbox for user's code

    try:
        compiled_code = compile(user_code, ""<string>"", ""exec"")      # Compile code to check Syntax/Indentations
        exec(compiled_code, user_code_namespace)                        # Runs code inside the namespace

        # Check & retrieve 'Solution' class, Check if execution method is defined
        if (solution_class := user_code_namespace.get(""Solution"")) is None:
            raise NameError(""Class 'Solution' is not defined"")
        if not hasattr(solution_class(), execution_method):
            raise AttributeError(f""Method '{{execution_method}}' is not defined."")
        
        # Test user's code, enumerate -> returns tuple (index of item, item)
        for index, test_case in enumerate(test_cases):
            arguments = test_case['arguments']
            expectedResult_type = test_case['expectedResult_type']
            expectedResult_value = test_case['expectedResult_value']
            
            # Start timer, Retrieve & Execute method, * Unpack arguments, Stop & Store longest exec time (ms)
            start_time = time.time()
            result = getattr(solution_class(), execution_method)(*arguments)
            elapsed_time = (time.time() - start_time) * 1000
            max_execution_time = max(max_execution_time, elapsed_time)

            # Check result type & value
            if type(result) != expectedResult_type:
                raise ResultTypeMismatchError(f'Test Case {{index+1}} failed. Result type: {{type(result)}}, Expected type: {{expectedResult_type}}')         
            if result != expectedResult_value:
                raise ResultValueMismatchError(f'Test Case {{index+1}} failed. Result value: {{result}}, Expected value: {{expectedResult_value}}')

        printResponse('Success', f'All tests passed, Max execution time: {{max_execution_time:.4f}} ms', '200')

    except (SyntaxError, IndentationError) as e:
        printResponse(type(e).__name__, f'{{e.msg}} at line {{e.lineno}}', '422')

    except (ResultTypeMismatchError, ResultValueMismatchError) as e:
        printResponse(type(e).__name__, str(e), '422')

    except Exception as e:
        printResponse(type(e).__name__, str(e), '422')

    except SystemExit:                                                  # Catches sys.exit()
        printResponse('SystemExit', 'Process attempted to exit', '422')

    except BaseException as e:                                          # MemoryError, KeyboardInterrupt...
        printResponse('CriticalError', str(e), '422')

if __name__ == '__main__':
    main()
    ";
        return driverCode;
    }
}