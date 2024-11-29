using MicroservicesAPI.Python.Services;
using MicroservicesAPI.Shared.DTOs;
using MicroservicesAPI.Shared.Entities;
using MicroservicesAPI.Shared.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesAPI.Python.Endpoints;


public static class PythonEndpoints
{
    public static void MapPythonEndpoints(this IEndpointRouteBuilder app)
    {
        // "DownstreamPathTemplate": "/Code/Python/get" --> pattern from app.Map
        // "UpstreamPathTemplate": "/Code/Python/get", --> pattern to access endpoint from eg. postman
        app.MapPost("Code/Python", SubmitUsersCode);
        app.MapGet("Code/Python/get", GetTestString);
    }

    
    private static async Task<Results<Ok<ResultResponseDto>, UnprocessableEntity<ResultResponseDto>, BadRequest<string>>> SubmitUsersCode(
        [FromBody] SubmittedSolutionDto submittedSolutionDto,
        [FromServices] TestingDataRepository testingDataRepo,
        [FromServices] PythonService pythonService)
    {
        try
        {
            TestingData testingData = await testingDataRepo.GetTestingDataByTaskIdAsync(submittedSolutionDto.TaskId); 
            ResultResponseDto resultResponseDto = await pythonService.ProcessUsersCode(submittedSolutionDto, testingData);
            
            if (resultResponseDto.ResultState is 
                "Success" or "ValueMismatchException" or 
                "TypeMismatchException") 
            {
                return TypedResults.Ok(resultResponseDto);
            }
            
            return TypedResults.UnprocessableEntity(resultResponseDto);
        }
        catch (Exception ex)
        {
            // --> error outside user's control, should be displayed to developer only
            return TypedResults.BadRequest(ex.ToString());
        }
    }
    
        
    public static async Task<IResult> GetTestString()
    {
        return  Results.Ok("Python String");
    }
    
    
}