using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HardshipApp.Repositories.Converters;

/// <summary>
/// Stores money as whole cents. SQLite has no decimal type, and a decimal stored as text
/// cannot be compared or ordered as a number.
/// </summary>
public sealed class MoneyToCentsConverter : ValueConverter<decimal, long>
{
    public MoneyToCentsConverter()
        : base(
            money => (long)Math.Round(money * 100m, MidpointRounding.ToEven),
            cents => cents / 100m)
    {
    }
}
