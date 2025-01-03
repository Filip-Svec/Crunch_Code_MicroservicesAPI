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
        [FromServices] IHttpClientFactory httpClientFactory // Add HttpClientFactory dependency
        )
    {
        try
        {
            // ?? only if there's an error in Db, should not happen
            CodingTask codingTask = await codingTaskRepo.GetCodingTaskByNameAsync(taskRequest.TaskName)
                                ?? throw new Exception($"Task: '{taskRequest.TaskName}' not found.");
            
            string language = taskRequest.Language.ToLower();
            string languageServiceUrl = $"http://microservicesapi.{language}/Code/{language}/templates?taskId={codingTask.Id}";

            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync(languageServiceUrl);
            
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to retrieve templates for language: {taskRequest.Language}. Status code: {response.StatusCode}");
            
            TemplateResponseDto templates = await response.Content.ReadFromJsonAsync<TemplateResponseDto>()
                            ?? throw new Exception($"Invalid response from {languageServiceUrl}");
            
            TaskResponseDto taskResponseDto = TaskResponseBuilder(codingTask, templates); //todo send template data here
            return TypedResults.Ok(taskResponseDto);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.ToString());
        }
    }
    
    private static TaskResponseDto TaskResponseBuilder(CodingTask codingTask, TemplateResponseDto templates)
    {
        return new TaskResponseDto
        {
            Id = codingTask.Id.ToString(),
            TaskName = codingTask.Name,
            Description = codingTask.Description,
            Examples = codingTask.Examples, 
            Constraints = codingTask.Constraints,
            Hints = codingTask.Hints,
            Template = templates.Template,    
            Solution = templates.Solution,    
            SolutionDesc = templates.SolutionDesc, 
        };
    }
    
}