using Microsoft.AspNetCore.Http.HttpResults;

namespace MicroservicesAPI.Python.Services;

public class PythonService
{
    public String InterpretUsersCode(String usersCode)
    {
        return usersCode.Replace('a', 'b');
    }
    
}