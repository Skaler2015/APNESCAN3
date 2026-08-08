using Xunit;

namespace ApneScan.Sdk.Tests.Asserts;

public static class CollectionAsserts
{
    public static void SameItems<T>(IEnumerable<T> first, IEnumerable<T> second)
    {
        Assert.Equal(first.OrderBy(x => x), second.OrderBy(x => x));
    }
}