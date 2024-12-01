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
            CodingTask codingTask = await codingTaskRepo.GetCodingTaskByNameAsync(taskRequest.TaskName); 
            PythonTemplates pythonTemplates = await pythonTemplateRepo.GetPythonTemplatesByTaskId(codingTask.Id.ToString()); 
            JavaScriptTemplates javaScriptTemplates = await javaScriptTemplateRepo.GetJsTemplatesByTaskId(codingTask.Id.ToString());
            TaskResponseDto taskResponseDto = null;//todo 
            return TypedResults.Ok(taskResponseDto);
        }
        catch (Exception ex)
        {
            // --> error outside user's code, should be displayed to developer only
            return TypedResults.BadRequest(ex.ToString());
        }
    }
    
}