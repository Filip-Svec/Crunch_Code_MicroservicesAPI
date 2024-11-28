using MicroservicesAPI.Python.Endpoints;
using MicroservicesAPI.Python.Services;
using MicroservicesAPI.Shared;
using MicroservicesAPI.Shared.Repository;
using MicroservicesAPI.Shared.Repository.Interfaces;
using MicroservicesAPI.Shared.Settings;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PythonService>();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton(typeof(IRepository<>), typeof(MongoRepository<>));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));
    var databaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value;
    return mongoClient.GetDatabase(databaseName);
});
builder.Services.AddScoped<TestingDataRepository>(sp =>
{
    var mongoDbContext = sp.GetRequiredService<MongoDbContext>(); // Resolve the context
    return new TestingDataRepository(mongoDbContext.TestingData); // Pass the TestingData collection
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
app.MapPythonEndpoints();

app.Run();