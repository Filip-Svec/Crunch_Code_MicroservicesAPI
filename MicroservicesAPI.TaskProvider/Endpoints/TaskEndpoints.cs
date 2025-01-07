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
        app.MapGet("Code/Task/names", RetrieveTaskNames);
    }

    private static async Task<Results<Ok<TaskNamesResponseDto>, BadRequest<string>>> RetrieveTaskNames(
        [FromServices] CodingTaskRepository codingTaskRepo
        )
    {
        try
        {
            List<string> taskNames = await codingTaskRepo.GetAllCodingTasksNames()
                                                        ?? throw new Exception($"There are no Task names available in CodingTasks collection");
            TaskNamesResponseDto taskNamesResponseDto = new TaskNamesResponseDto
            {
                AllTaskNames = taskNames
            };
            return TypedResults.Ok(taskNamesResponseDto);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.ToString());
        }
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
            
            // Build URL & pass coding task ID
            string language = taskRequest.Language.ToLower();
            string languageServiceUrl = $"http://microservicesapi.{language}/Code/{language}/templates?taskId={codingTask.Id}";

            // Request data (templates) from GET endpoint in language microservice
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync(languageServiceUrl);
            
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to retrieve templates for {taskRequest.Language}. Status code: {response.StatusCode}");
            
            // Load data
            TemplateResponseDto templates = await response.Content.ReadFromJsonAsync<TemplateResponseDto>()
                            ?? throw new Exception($"Invalid response from {languageServiceUrl}");
            
            // Build Task Response
            TaskResponseDto taskResponseDto = TaskResponseBuilder(codingTask, templates); 
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