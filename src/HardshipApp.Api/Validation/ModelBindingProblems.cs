using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HardshipApp.Api.Validation;

/// <summary>
/// Rewrites model binding failures before they reach the caller.
/// </summary>
internal static class ModelBindingProblems
{
    private const string UnreadableValue = "The value supplied is not valid for this field.";

    public static IActionResult ToProblem(ActionContext context)
    {
        List<KeyValuePair<string, ModelStateEntry?>> entries = context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .Select(entry => new KeyValuePair<string, ModelStateEntry?>(entry.Key, entry.Value))
            .ToList();

        // When the body named the offending field, the whole-parameter error ("the request
        // field is required") adds nothing a caller can act on.
        bool hasFieldPaths = entries.Exists(entry => entry.Key.StartsWith("$.", StringComparison.Ordinal));

        var errors = entries
            .Where(entry => !hasFieldPaths || entry.Key.StartsWith("$.", StringComparison.Ordinal))
            .SelectMany(entry => entry.Value!.Errors.Select(error => (
                Field: ToFieldName(entry.Key),
                Message: ToMessage(error, isJsonPath: entry.Key.StartsWith("$.", StringComparison.Ordinal)))))
            .ToList();

        return new BadRequestObjectResult(ValidationProblems.Create(context.HttpContext, errors));
    }

    /// <summary>Turns the binder's <c>$.dateOfBirth</c> into the <c>DateOfBirth</c> the validators use.</summary>
    private static string ToFieldName(string key)
    {
        string name = key.StartsWith("$.", StringComparison.Ordinal) ? key[2..] : key;

        return name.Length == 0 ? name : char.ToUpperInvariant(name[0]) + name[1..];
    }

    /// <summary>
    /// An error on a JSON path is the serialiser's, and its text names the .NET type it
    /// failed to build — an internal detail, and no use to whoever filled in the form.
    /// </summary>
    private static string ToMessage(ModelError error, bool isJsonPath) =>
        isJsonPath || error.Exception is not null || string.IsNullOrWhiteSpace(error.ErrorMessage)
            ? UnreadableValue
            : error.ErrorMessage;
}
