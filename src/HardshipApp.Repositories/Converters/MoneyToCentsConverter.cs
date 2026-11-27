using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HardshipApp.Repositories.Converters;

/// <summary>
/// Stores money as a whole number of cents.
/// </summary>
/// <remarks>
/// SQLite has no decimal type. Left alone, EF Core stores a <see cref="decimal"/> as TEXT and
/// says so in a model warning, because text cannot be compared or ordered as a number —
/// '900.00' sorts before '90.00'. Integer cents keeps arithmetic exact, keeps ORDER BY and
/// range filters correct in SQL, and is how money is usually stored regardless of the engine.
/// <para>
/// Rounding is banker's rounding, matching <see cref="decimal"/>'s own default, so a value
/// that survives a round trip is the value that went in.
/// </para>
/// </remarks>
public sealed class MoneyToCentsConverter : ValueConverter<decimal, long>
{
    public MoneyToCentsConverter()
        : base(
            money => (long)Math.Round(money * 100m, MidpointRounding.ToEven),
            cents => cents / 100m)
    {
    }
}
