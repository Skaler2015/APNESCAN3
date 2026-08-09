namespace ApneScan.App.Tests.Targets;

public class MacAppTestTarget : IAppTestTarget
{
    public AppTestExe Console => GetAppTestExe("console");
    public AppTestExe Gui => GetAppTestExe(null);
    public AppTestExe Worker => GetAppTestExe("worker");
    public AppTestExe Server => GetAppTestExe("server");
    public bool IsWindows => false;

    private AppTestExe GetAppTestExe(string argPrefix)
    {
        return new AppTestExe(
            Path.Combine(AppTestHelper.SolutionRoot, "ApneScan.App.Mac", "bin", "Debug", "net10.0-macos"),
            Path.Combine("ApneScan.app", "Contents", "MacOS", "ApneScan"),
            argPrefix);
    }

    public override string ToString() => "Mac";
}