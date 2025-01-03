using MicroservicesAPI.Shared;
using MicroservicesAPI.Shared.Repository;
using MicroservicesAPI.Shared.Repository.Interfaces;
using MicroservicesAPI.Shared.Settings;
using MicroservicesAPI.TaskProvider.Endpoints;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton(typeof(IRepository<>), typeof(MongoRepository<>));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));
    var databaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value;
    
    return mongoClient.GetDatabase(databaseName);
});

builder.Services.AddScoped<CodingTaskRepository>(sp =>
{
    var mongoDbContext = sp.GetRequiredService<MongoDbContext>(); 
    return new CodingTaskRepository(mongoDbContext.CodingTasks); // pass the TestingData collection
});
builder.Services.AddScoped<PythonTemplateRepository>(sp =>
{
    var mongoDbContext = sp.GetRequiredService<MongoDbContext>(); 
    return new PythonTemplateRepository(mongoDbContext.PythonTemplates); // pass the TestingData collection
});
builder.Services.AddScoped<JavaScriptTemplateRepository>(sp =>
{
    var mongoDbContext = sp.GetRequiredService<MongoDbContext>(); 
    return new JavaScriptTemplateRepository(mongoDbContext.JavaScriptTemplates); // pass the TestingData collection
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

//Minimal APIs -- requires mapping of all Endpoint.cs files in specific project
// - or the use of extension Carter
app.MapTaskProviderEndpoints();

app.Run();