using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HardshipApp.Api.Validation;

/// <summary>
/// Validates any action argument that has a registered validator, and turns failures into
/// the same ValidationProblemDetails shape the framework produces for binding errors.
/// </summary>
internal sealed class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (object? argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            Type validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            ValidationResult result = await validator.ValidateAsync(
                new ValidationContext<object>(argument),
                context.HttpContext.RequestAborted);

            if (result.IsValid)
            {
                continue;
            }

            context.Result = new BadRequestObjectResult(ValidationProblems.Create(
                context.HttpContext,
                result.Errors.Select(failure => (failure.PropertyName, failure.ErrorMessage))));

            return;
        }

        await next();
    }
}
