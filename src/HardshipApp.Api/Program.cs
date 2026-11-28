using System.Reflection;
using FluentValidation;
using HardshipApp.Api;
using HardshipApp.Api.Validation;
using HardshipApp.Repositories;
using HardshipApp.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options => options.Filters.Add<ValidationFilter>())
    .ConfigureApiBehaviorOptions(options =>
        options.InvalidModelStateResponseFactory = ModelBindingProblems.ToProblem);
builder.Services.AddOpenApi(options => options.AddSchemaTransformer<DecimalSchemaTransformer>());

builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.TryAddSingleton(TimeProvider.System);

builder.Services.AddApplicationServices();
builder.Services.AddPersistence(
    builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' is not configured."));

WebApplication app = builder.Build();

app.UseExceptionHandler();

// Routing and the framework answer some requests before any of our code runs — an unknown
// path, a route constraint that did not match. Without this they come back with an empty
// body, and a client that expects problem details has nothing to read.
app.UseStatusCodePages();

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

// Exposed so the integration tests can host the real application.
public partial class Program;
