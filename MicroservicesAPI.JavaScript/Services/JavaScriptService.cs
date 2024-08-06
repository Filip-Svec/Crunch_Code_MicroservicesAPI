namespace MicroservicesAPI.JavaScript.Services;

public class JavaScriptService
{
    public String InterpretUsersCode(String usersCode)
    {
        return usersCode.Replace('a', 'c');
    }
}