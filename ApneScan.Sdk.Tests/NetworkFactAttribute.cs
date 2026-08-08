using Xunit;

namespace ApneScan.Sdk.Tests;

public class NetworkFactAttribute : FactAttribute
{
    public NetworkFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("ApneScan_TEST_NONETWORK") == "1")
        {
            Skip = "Running without network access, skipping ESCL tests";
        }
    }
}