using MicroservicesAPI.Common.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MicroservicesAPI.Python.Services;

public class PythonService
{
    public String InterpretUsersCode(SubmittedCodeDto submittedCodeDto)
    {
        return submittedCodeDto.UsersCode.Replace('a', 'b');
    }
    
}