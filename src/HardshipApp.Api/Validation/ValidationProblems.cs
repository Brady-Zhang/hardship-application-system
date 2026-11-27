using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HardshipApp.Api.Validation;

internal static class ValidationProblems
{
    /// <summary>
    /// Builds the response through <see cref="ProblemDetailsFactory"/> so a validation failure
    /// and a model binding failure come back in exactly the same shape.
    /// </summary>
    public static ValidationProblemDetails Create(
        HttpContext httpContext,
        IEnumerable<(string Field, string Message)> errors)
    {
        ModelStateDictionary modelState = new();

        foreach ((string field, string message) in errors)
        {
            modelState.AddModelError(field, message);
        }

        return httpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>()
            .CreateValidationProblemDetails(httpContext, modelState, StatusCodes.Status400BadRequest);
    }
}
