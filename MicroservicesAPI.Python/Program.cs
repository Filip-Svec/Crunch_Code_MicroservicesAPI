using MicroservicesAPI.Python.Endpoints;
using MicroservicesAPI.Python.Services;
using MicroservicesAPI.Shared;
using MicroservicesAPI.Shared.Repository;
using MicroservicesAPI.Shared.Repository.Interfaces;
using MicroservicesAPI.Shared.Settings;
using MongoDB.Driver;

// Instance of WebApplicationBuilder --> configuration, logging, dependency inj
// Loads config from appsettings.json,  registers services
var builder = WebApplication.CreateBuilder(args);   

// Add services to the container
// Scoped - new instance per request; Singleton - one instance within the app
builder.Services.AddEndpointsApiExplorer();     // Used for generating API metadata & docs
builder.Services.AddScoped<PythonService>();    
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings")); // Configure Mongo
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton(typeof(IRepository<>), typeof(MongoRepository<>));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));    // Creates client
    var databaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value;  
    return mongoClient.GetDatabase(databaseName);   // Returns instance of the database
});
builder.Services.AddScoped<TestingDataRepository>(sp =>
{
    var mongoDbContext = sp.GetRequiredService<MongoDbContext>(); 
    return new TestingDataRepository(mongoDbContext.TestingData);   // Pass the TestingData collection
});
builder.Services.AddScoped<PythonTemplateRepository>(sp =>
{
    var mongoDbContext = sp.GetRequiredService<MongoDbContext>(); 
    return new PythonTemplateRepository(mongoDbContext.PythonTemplates); 
});
var app = builder.Build();      // Builds the app with the settings above

app.UseHttpsRedirection();      // Redirects all HTTP requests to HTTPS

// Minimal APIs -- requires mapping of all Endpoint.cs files in specific project
app.MapPythonEndpoints();

app.Run();      // Starts the app duhh :D