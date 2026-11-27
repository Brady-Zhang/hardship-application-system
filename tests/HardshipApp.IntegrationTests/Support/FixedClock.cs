namespace HardshipApp.IntegrationTests.Support;

/// <summary>
/// A clock that reports a time the test chose, and only moves when the test moves it.
/// </summary>
/// <remarks>
/// Six lines instead of a testing package: the only member anything here calls is
/// <see cref="GetUtcNow"/>, and a fake with no moving parts cannot be the reason a test fails.
/// </remarks>
internal sealed class FixedClock(DateTimeOffset now) : TimeProvider
{
    public DateTimeOffset Now { get; set; } = now;

    public override DateTimeOffset GetUtcNow() => Now;
}
