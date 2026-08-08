using ApneScan.App.Tests.Targets;
using ApneScan.Remoting;
using ApneScan.Sdk.Tests;
using Xunit;

namespace ApneScan.App.Tests;

public class GuiAppTests : ContextualTests
{
    [GuiTheory]
    [ClassData(typeof(AppTestData))]
    public void CreatesWindow(IAppTestTarget target)
    {
        var process = AppTestHelper.StartGuiProcess(target.Gui, FolderPath);
        try
        {
            if (target.IsWindows)
            {
                AppTestHelper.WaitForVisibleWindow(process);
                Assert.Equal("ApneScan - Not Another PDF Scanner", process.MainWindowTitle);
                Assert.True(process.CloseMainWindow());
            }
            else
            {
                var helper = ProcessCoordinator.CreateDefault();
                Assert.True(helper.CloseWindow(process, 5000));
            }
            Assert.True(process.WaitForExit(5000));
            AppTestHelper.AssertNoErrorLog(FolderPath);
        }
        finally
        {
            AppTestHelper.Cleanup(process);
        }
    }
}