using MicroservicesAPI.JavaScript.Services;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesAPI.JavaScript.Controllers;


[ApiController]
[Route("Code")]
public class JavaScriptController
{
    private readonly JavaScriptService _javaScriptService;
    public JavaScriptController(JavaScriptService javaScriptService)
    {
        _javaScriptService = javaScriptService;
    }

    [HttpPost]
    [Route("JavaScript")]
    public String SubmitUsersCode([FromBody] String usersCode)
    {
        return  _javaScriptService.InterpretUsersCode(usersCode);
    }
    
    [HttpGet]
    [Route("JavaScript/get")]
    public String GetTestString()
    {
        return  "JavaScript String";
    }
}