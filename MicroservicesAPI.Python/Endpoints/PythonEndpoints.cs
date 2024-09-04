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

    
    public static async Task<IResult> SubmitUsersCode(
        [FromBody] SubmittedCodeDto submittedCode,
        PythonService pythonService)
    {
        ResultState resultState = await pythonService.ProcessUsersCode(submittedCode);
        
        if (resultState != ResultState.Unknown)
        {
            return Results.Ok(pythonService.BuildResponseDto(resultState));
        }
        else
        {
            return Results.BadRequest(pythonService.BuildResponseDto(resultState));
        }
        
    }
    
    public static async Task<IResult> GetTestString()
    {
        return  Results.Ok("Python String");
    }
    
    
}