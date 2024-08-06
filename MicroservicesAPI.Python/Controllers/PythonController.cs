using MicroservicesAPI.Python.Services;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesAPI.Python.Controllers;


[ApiController]
[Route("Code")]
public class PythonController : ControllerBase
{
    private readonly PythonService _pythonService;
    public PythonController(PythonService pythonService)
    {
        _pythonService = pythonService;
    }

    [HttpPost]
    [Route("Python")]
    public String SubmitUsersCode([FromBody] String usersCode)
    {
        return  _pythonService.InterpretUsersCode(usersCode);
    }
    
    [HttpGet]
    [Route("Python/get")]
    public String GetTestString()
    {
        return  "Python String";
    }
    
    
}