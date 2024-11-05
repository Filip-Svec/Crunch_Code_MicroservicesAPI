using MicroservicesAPI.Python.Services;
using MicroservicesAPI.Shared;
using MicroservicesAPI.Shared.DTOs;
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

    
    private static async Task<Results<Ok<ResultResponseDto>, UnprocessableEntity<ResultResponseDto>>> SubmitUsersCode(
        [FromBody] SubmittedSolutionDto submittedSolution,
        PythonService pythonService)
    {
        ResultResponseDto resultResponseDto = await pythonService.ProcessUsersCode(submittedSolution);
        
        if (resultResponseDto.ResultState is 
            ResultState.Success or ResultState.ValueMismatch or 
            ResultState.TypeMismatch) 
        {
            return TypedResults.Ok(resultResponseDto);
        }
        else
        {
            return TypedResults.UnprocessableEntity(resultResponseDto);
        }
    }
    
    public static async Task<IResult> GetTestString()
    {
        return  Results.Ok("Python String");
    }
    
    
}