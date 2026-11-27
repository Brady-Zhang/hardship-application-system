using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HardshipApp.Repositories.Converters;

/// <summary>
/// Stores an instant as UTC ticks.
/// </summary>
/// <remarks>
/// The same problem as money, for the same reason. SQLite stores a
/// <see cref="DateTimeOffset"/> as text and then refuses to order by it at all —
/// "SQLite does not support expressions of type 'DateTimeOffset' in ORDER BY clauses" — so a
/// list sorted newest first could only have been sorted in the client, after fetching every
/// row.
/// <para>
/// Storing a <see cref="DateTime"/> instead would sort, but it loses the offset and comes back
/// with <see cref="DateTimeKind.Unspecified"/>, which serialises without a 'Z' and is then
/// read by the browser as local time. Ticks keep full precision, sort as integers, and leave
/// no room for a timezone to be guessed.
/// </para>
/// </remarks>
public sealed class InstantToUtcTicksConverter : ValueConverter<DateTimeOffset, long>
{
    public InstantToUtcTicksConverter()
        : base(
            instant => instant.UtcDateTime.Ticks,
            ticks => new DateTimeOffset(ticks, TimeSpan.Zero))
    {
    }
}
