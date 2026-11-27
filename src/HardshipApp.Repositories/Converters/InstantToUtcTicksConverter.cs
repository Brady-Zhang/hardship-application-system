using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HardshipApp.Repositories.Converters;

/// <summary>
/// Stores an instant as UTC ticks. SQLite refuses to order by a DateTimeOffset, and storing
/// a DateTime instead loses the kind and is then read as local time by the browser.
/// </summary>
public sealed class InstantToUtcTicksConverter : ValueConverter<DateTimeOffset, long>
{
    public InstantToUtcTicksConverter()
        : base(
            instant => instant.UtcDateTime.Ticks,
            ticks => new DateTimeOffset(ticks, TimeSpan.Zero))
    {
    }
}
