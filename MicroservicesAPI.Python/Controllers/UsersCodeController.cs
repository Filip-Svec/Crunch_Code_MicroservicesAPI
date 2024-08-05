using MicroservicesAPI.Python.Services;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesAPI.Python.Controllers;


[ApiController]
[Route("[controller]")]
public class UsersCodeController : ControllerBase
{
    private readonly UsersCodeService _usersCodeService;
    public UsersCodeController(UsersCodeService usersCodeService)
    {
        _usersCodeService = usersCodeService;
    }

    [HttpPost]
    [Route("/Code/Submit")]
    public String SubmitUsersCode([FromBody] String usersCode)
    {
        var result =  _usersCodeService.InterpretUsersCode(usersCode);
        return result;
    }
    
    
}