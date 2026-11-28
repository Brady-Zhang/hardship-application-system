using System.Reflection;
using FluentValidation;
using HardshipApp.Api;
using HardshipApp.Api.Validation;
using HardshipApp.Repositories;
using HardshipApp.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options => options.Filters.Add<ValidationFilter>())
    .ConfigureApiBehaviorOptions(options =>
        options.InvalidModelStateResponseFactory = ModelBindingProblems.ToProblem);
builder.Services.AddOpenApi(options =>
{
    options.AddSchemaTransformer<DecimalSchemaTransformer>();

    // The assembly name is not a title. This is what the reference page and anyone reading
    // the document see.
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Hardship Application API";
        return Task.CompletedTask;
    });
});

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

    // An API reference to read and to call the endpoints from, served off the same document
    // the client's types are generated from. Development only: it is a tool for whoever is
    // working on this, not a public page.
    app.MapScalarApiReference(options => options.WithTitle("Hardship Application API"));
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
