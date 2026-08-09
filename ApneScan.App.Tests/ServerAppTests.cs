using ApneScan.App.Tests.Targets;
using ApneScan.Remoting;
using ApneScan.Sdk.Tests;
using Xunit;

namespace ApneScan.App.Tests;

public class ServerAppTests : ContextualTests
{
    [GuiTheory]
    [ClassData(typeof(AppTestData))]
    public void StartAndStopServer(IAppTestTarget target)
    {
        var process = AppTestHelper.StartProcess(target.Server, FolderPath);
        try
        {
            var helper = ProcessCoordinator.CreateDefault();
            Assert.True(helper.StopSharingServer(process, 5000));
            Assert.True(process.WaitForExit(5000));
            AppTestHelper.AssertNoErrorLog(FolderPath);
        }
        finally
        {
            AppTestHelper.Cleanup(process);
        }
    }
}