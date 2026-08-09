using System.Collections;
using ApneScan.App.Tests.Targets;

namespace ApneScan.App.Tests.Appium;

public class AppiumTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
#if NET6_0_OR_GREATER
        if (OperatingSystem.IsWindows())
        {
            yield return new object[] { new WindowsAppTestTarget() };
        }
        else if (OperatingSystem.IsMacOS())
        {
            // No Appium impl yet
        }
        else if (OperatingSystem.IsLinux())
        {
            // No Appium impl yet
        }
#else
        yield return new object[] { new WindowsAppTestTarget() };
#endif
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}