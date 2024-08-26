using MicroservicesAPI.Python.Services;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesAPI.Python.Controllers;


public static class PythonEndpoints
{
    public static void MapPythonEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("code/python", SubmitUsersCode);
        app.MapGet("code/python/get", GetTestString);
    }

    
    public static async Task<IResult> SubmitUsersCode(
        [FromBody] String usersCode,
        PythonService pythonService)
    {
        return  Results.Ok(pythonService.InterpretUsersCode(usersCode));
    }
    
    public static async Task<IResult> GetTestString()
    {
        return  Results.Ok("Python String");
    }
    
    
}