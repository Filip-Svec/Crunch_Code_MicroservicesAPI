namespace MicroservicesAPI.Shared.Settings;

public class MongoDbSettings
{
    // Configured by Program.cs that acquired them from appsettings.json
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}