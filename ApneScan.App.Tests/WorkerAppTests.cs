using GrpcDotNetNamedPipes;
using ApneScan.App.Tests.Targets;
using ApneScan.Remoting.Worker;
using ApneScan.Sdk.Tests;
using Xunit;

namespace ApneScan.App.Tests;

public class WorkerAppTests : ContextualTests
{
    [Theory]
    [ClassData(typeof(AppTestData))]
    public void CreatesPipeServer(IAppTestTarget target)
    {
        var process = AppTestHelper.StartProcess(target.Worker, FolderPath, Process.GetCurrentProcess().Id.ToString());
        try
        {
            Assert.Equal("ready", process.StandardOutput.ReadLine());
            string pipeName = $"ApneScan.Worker.{process.Id}";
            var client = new WorkerServiceAdapter(new NamedPipeChannel(".", pipeName));
            client.Init(FolderPath);
            AppTestHelper.AssertNoErrorLog(FolderPath);
        }
        finally
        {
            AppTestHelper.Cleanup(process);
        }
    }
}