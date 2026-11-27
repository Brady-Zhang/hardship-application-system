using HardshipApp.Api;
using HardshipApp.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddPersistence(
    builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' is not configured."));

WebApplication app = builder.Build();

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
