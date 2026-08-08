using System.Collections;

namespace ApneScan.App.Tests.Verification;

public class InstallDirTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ApneScan_TEST_VERIFY")))
        {
            yield break;
        }
#if NET6_0_OR_GREATER
        if (OperatingSystem.IsWindows())
        {
            yield return new object[] { Environment.GetEnvironmentVariable("ApneScan_TEST_ROOT") };
        }
        else if (OperatingSystem.IsMacOS())
        {
            // No tests yet
        }
        else if (OperatingSystem.IsLinux())
        {
            // No tests yet
        }
#else
        yield return new object[] { Environment.GetEnvironmentVariable("ApneScan_TEST_ROOT") };
#endif
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}