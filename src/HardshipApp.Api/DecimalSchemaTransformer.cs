using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HardshipApp.Api;

/// <summary>
/// Describes a decimal as a plain JSON number.
/// </summary>
/// <remarks>
/// The default schema is <c>["number", "string"]</c>, because the serialiser will read a
/// decimal from a quoted string. It never writes one, so the union is inaccurate for every
/// response and it leaks into the generated client as <c>number | string</c> on every money
/// field.
/// </remarks>
internal sealed class DecimalSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type == typeof(decimal) || context.JsonTypeInfo.Type == typeof(decimal?))
        {
            schema.Type = JsonSchemaType.Number;
            schema.Pattern = null;
        }

        return Task.CompletedTask;
    }
}
