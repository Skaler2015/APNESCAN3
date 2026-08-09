using Xunit;

namespace ApneScan.Sdk.Tests.Asserts;

public class DateAsserts
{
    public static void Recent(TimeSpan recency, DateTime? actual)
    {
        Assert.NotNull(actual);
        Assert.InRange(actual.Value, actual.Value - recency, DateTime.Now);
    }
}