using MicroservicesAPI.Shared.Entities;
using MicroservicesAPI.Shared.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MicroservicesAPI.Shared;

// Class explanation:
// --> defines logic for connecting and interacting with the MongoDB
// --> hub for accessing the collections
public class MongoDbContext
{
    // Instance of the Db, readonly --> initialized once (not changing)
    private readonly IMongoDatabase _database;  

    // IOptions --> configuration pattern, load and pass strongly typed settings (connection string)
    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        Console.WriteLine($"MongoDB Connection: {settings.Value.ConnectionString}");
        Console.WriteLine($"Database: {settings.Value.DatabaseName}");
        // connection to the Db server using connection string
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    // Loading Collections
    public IMongoCollection<CodingTask> CodingTasks => _database.GetCollection<CodingTask>("CodingTasks");
    public IMongoCollection<TestingData> TestingData => _database.GetCollection<TestingData>("TestingData");
    public IMongoCollection<PythonTemplates> PythonTemplates => _database.GetCollection<PythonTemplates>("PythonTemplates");
    public IMongoCollection<JavaScriptTemplates> JavaScriptTemplates => _database.GetCollection<JavaScriptTemplates>("JavaScriptTemplates");
}