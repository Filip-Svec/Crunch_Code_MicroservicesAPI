using MicroservicesAPI.Common.DTOs;
using MicroservicesAPI.Python.Services;
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
        try
        {
            pythonService.InterpretUsersCode(submittedCode);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Results.BadRequest(e.Message);
        }
        return Results.Ok("fine");
    }
    
    public static async Task<IResult> GetTestString()
    {
        return  Results.Ok("Python String");
    }
    
    
}