using HardshipApp.Api;
using HardshipApp.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddPersistence(
    builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' is not configured."));

WebApplication app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // Off in development: the Vite proxy cannot follow the 307.
    app.UseHttpsRedirection();
}

app.MapControllers();

await app.ApplyMigrationsAsync();

await app.RunAsync();
