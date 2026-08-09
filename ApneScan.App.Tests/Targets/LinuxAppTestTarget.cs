using System.Runtime.InteropServices;

namespace ApneScan.App.Tests.Targets;

public class LinuxAppTestTarget : IAppTestTarget
{
    public AppTestExe Console => GetAppTestExe("console");
    public AppTestExe Gui => GetAppTestExe(null);
    public AppTestExe Worker => GetAppTestExe("worker");
    public AppTestExe Server => GetAppTestExe("server");
    public bool IsWindows => false;

    private AppTestExe GetAppTestExe(string argPrefix)
    {
        var runtime = RuntimeInformation.OSArchitecture == Architecture.Arm64 ? "linux-arm64" : "linux-x64";
        return new AppTestExe(
            Path.Combine(AppTestHelper.SolutionRoot, "ApneScan.App.Gtk", "bin", "Debug", "net10.0", runtime),
            "apnescan",
            argPrefix);
    }

    public override string ToString() => "Linux";
}