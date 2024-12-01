using MicroservicesAPI.Shared.DTOs;
using MicroservicesAPI.Shared.Entities;
using MicroservicesAPI.Shared.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesAPI.TaskProvider.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskProviderEndpoints(this IEndpointRouteBuilder app)
    {
        // "DownstreamPathTemplate": "/Code/Python/get" --> pattern from app.Map
        // "UpstreamPathTemplate": "/Code/Python/get", --> pattern to access endpoint from eg. postman
        app.MapGet("Code/Task", RetrieveTaskData);
    }
    
    private static async Task<Results<Ok<TaskResponseDto>, BadRequest<string>>> RetrieveTaskData(
        [FromBody] TaskRequestDto taskRequest,
        [FromServices] CodingTaskRepository codingTaskRepo,
        [FromServices] PythonTemplateRepository pythonTemplateRepo,
        [FromServices] JavaScriptTemplateRepository javaScriptTemplateRepo
        )
    {
        try
        {
            CodingTask codingTask = await codingTaskRepo.GetCodingTaskByNameAsync(taskRequest.TaskName)
                                ?? throw new Exception($"Task: '{taskRequest.TaskName}' not found.");
            PythonTemplates pythonTemplates = await pythonTemplateRepo.GetPythonTemplatesByTaskId(codingTask.Id)
                                ?? throw new Exception($"Python Template with coding task Id: '{codingTask.Id.ToString()}' not found.");
            JavaScriptTemplates javaScriptTemplates = await javaScriptTemplateRepo.GetJsTemplatesByTaskId(codingTask.Id) 
                                ?? throw new Exception($"JavaScript Template with coding task Id: '{codingTask.Id.ToString()}' not found.");
            TaskResponseDto taskResponseDto = TaskResponseBuilder(codingTask, pythonTemplates, javaScriptTemplates);
            return TypedResults.Ok(taskResponseDto);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.ToString());
        }
    }
    
    private static TaskResponseDto TaskResponseBuilder(CodingTask codingTask, PythonTemplates pythonTemplates, JavaScriptTemplates javaScriptTemplates)
    {
        return new TaskResponseDto
        {
            Id = codingTask.Id.ToString(),
            TaskName = codingTask.Name,
            Description = codingTask.Description,
            Examples = codingTask.Examples, 
            Constraints = codingTask.Constraints,
            Hints = codingTask.Hints,
            PythonTemplate = pythonTemplates.TemplateCode,
            PythonSolution = pythonTemplates.SolutionCode,
            PythonSolutionDesc = pythonTemplates.SolutionCodeDescription,
            JavaScriptTemplate = javaScriptTemplates.TemplateCode,
            JavaScriptSolution = javaScriptTemplates.SolutionCode,
            JavaScriptSolutionDesc = javaScriptTemplates.SolutionCodeDescription
        };
    }
    
}