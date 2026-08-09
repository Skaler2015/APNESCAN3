using Xunit;

namespace ApneScan.App.Tests;

public class GuiTheoryAttribute : TheoryAttribute
{
    public GuiTheoryAttribute()
    {
        if (Environment.GetEnvironmentVariable("ApneScan_TEST_NOGUI") == "1")
        {
            Skip = "Running in headless mode, skipping GUI tests";
        }
    }
}