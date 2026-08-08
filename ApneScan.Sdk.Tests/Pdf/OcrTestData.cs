using System.Collections;
using ApneScan.Ocr;

namespace ApneScan.Sdk.Tests.Pdf;

public class OcrTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new OcrTestConfig(new StorageConfig.Memory(), null) };
        yield return new object[] { new OcrTestConfig(new StorageConfig.File(), null) };
        yield return new object[] { new OcrTestConfig(new StorageConfig.Memory(), new OcrParams("eng", OcrMode.Default, 60)) };
        yield return new object[] { new OcrTestConfig(new StorageConfig.File(), new OcrParams("eng", OcrMode.Default, 60)) };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}