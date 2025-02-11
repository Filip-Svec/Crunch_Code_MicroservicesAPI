using MicroservicesAPI.Python.Services;
using MicroservicesAPI.Shared.DTOs;
using MicroservicesAPI.Shared.Entities;
using MicroservicesAPI.Shared.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MicroservicesAPI.Python.Endpoints;


public static class PythonEndpoints
{
    public static void MapPythonEndpoints(this IEndpointRouteBuilder app)
    {
        // "DownstreamPathTemplate": "/Code/Python/get" --> pattern from app.Map
        // "UpstreamPathTemplate": "/Code/Python/get", --> pattern to access endpoint from eg. postman
        app.MapPost("Code/Python", SubmitUsersCode);
        app.MapGet("Code/python/templates", GetTemplate);
    }
    
    private static async Task<Results<Ok<ResultResponseDto>, UnprocessableEntity<ResultResponseDto>, BadRequest<string>, BadRequest<ResultResponseDto>>> SubmitUsersCode(
        [FromBody] SubmittedSolutionDto submittedSolutionDto,
        [FromServices] TestingDataRepository testingDataRepo,
        [FromServices] PythonService pythonService)
    {
        try
        {
            TestingData testingData = await testingDataRepo.GetTestingDataByTaskIdAsync(submittedSolutionDto.TaskId); 
            ResultResponseDto resultResponseDto = await pythonService.ProcessUsersCode(submittedSolutionDto, testingData);
            
            if (resultResponseDto.ResultStatusCode is "200") 
            {
                return TypedResults.Ok(resultResponseDto);
            }
            if (resultResponseDto.ResultStatusCode is "422")
            {
                return TypedResults.UnprocessableEntity(resultResponseDto);
            }
            return TypedResults.BadRequest(resultResponseDto);
            
        }
        catch (Exception ex)
        {
            // --> error outside user's code, should be displayed to developer only
            return TypedResults.BadRequest(ex.ToString());
        }
    }

    private static async Task<Results<Ok<TemplateResponseDto>, BadRequest<string>>> GetTemplate(
        [FromQuery] string taskId,
        [FromServices] PythonTemplateRepository pythonTemplateRepo)
    {
        try
        {
            // Fetch python templates
            PythonTemplates pythonTemplates = await pythonTemplateRepo.GetPythonTemplatesByTaskId(new ObjectId(taskId)) 
                            ?? throw new Exception($"Python Template with coding task Id: '{taskId}' not found.");
            
            // Build Template Response & return
            return TypedResults.Ok(new TemplateResponseDto
            {
                Template = pythonTemplates.TemplateCode,
                Solution = pythonTemplates.SolutionCode,
                SolutionDesc = pythonTemplates.SolutionCodeDescription
            });
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.ToString());
        }
        
    }

}