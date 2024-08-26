using MicroservicesAPI.Python.Controllers;
using MicroservicesAPI.Python.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PythonService>();

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