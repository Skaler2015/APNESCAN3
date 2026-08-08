using System.Collections;

namespace ApneScan.Sdk.Tests;

public class StorageAwareTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new StorageConfig.Memory() };
        yield return new object[] { new StorageConfig.File() };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}