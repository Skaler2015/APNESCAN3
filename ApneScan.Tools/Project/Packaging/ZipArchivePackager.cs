using System.IO.Compression;
using ApneScan.Tools.Project.Targets;

namespace ApneScan.Tools.Project.Packaging;

public static class ZipArchivePackager
{
    public static void PackageZip(Func<PackageInfo> pkgInfoFunc, Platform platform, bool noSign)
    {
        string arch = platform == Platform.WinArm64 ? "arm64" : "x64";

        Output.Verbose("Building binaries");
        if (platform != Platform.WinArm64)
        {
            Cli.Run("dotnet", "clean ApneScan.App.Worker -c Release");
        }
        Cli.Run("dotnet", $"clean ApneScan.App.WinForms -r win-{arch} -c Release");
        Cli.Run("dotnet", $"clean ApneScan.App.Console -r win-{arch} -c Release");
        if (platform != Platform.WinArm64)
        {
            Cli.Run("dotnet",
                "publish ApneScan.App.Worker -c Release /p:DebugType=None /p:DebugSymbols=false /p:DefineConstants=ZIP");
        }
        Cli.Run("dotnet", $"publish ApneScan.App.WinForms -r win-{arch} -c Release /p:DebugType=None /p:DebugSymbols=false /p:DefineConstants=ZIP");
        Cli.Run("dotnet", $"publish ApneScan.App.Console -r win-{arch} -c Release /p:DebugType=None /p:DebugSymbols=false /p:DefineConstants=ZIP");
        Cli.Run("dotnet", "build ApneScan.App.PortableLauncher -c Release");

        var pkgInfo = pkgInfoFunc();
        if (!noSign)
        {
            Output.Verbose("Signing contents");
            WindowsSigning.SignContents(pkgInfo);
        }

        var zipPath = pkgInfo.GetPath("zip");
        Output.Info($"Packaging zip archive: {zipPath}");

        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }

        var portableExe = Path.Combine(Paths.SolutionRoot, "ApneScan.App.PortableLauncher", "bin", "Release", "net462",
            "ApneScan.Portable.exe");
        if (!File.Exists(portableExe))
        {
            throw new Exception($"Could not find portable exe: {portableExe}");
        }
        if (!noSign)
        {
            Output.Verbose("Signing ApneScan.Portable.exe");
            WindowsSigning.SignFile(portableExe);
        }
        
        using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
        foreach (var file in pkgInfo.Files)
        {
            var destPath = Path.Combine("App", file.DestPath);
            Output.Verbose($"Compressing {destPath}");
            archive.CreateEntryFromFile(file.SourcePath, destPath);
        }
        Output.Verbose("Creating Data/");
        archive.CreateEntry("Data/");
        Output.Verbose("Compressing ApneScan.Portable.exe");
        archive.CreateEntryFromFile(portableExe, "ApneScan.Portable.exe");
        
        Output.OperationEnd($"Packaged zip archive: {zipPath}");
    }
}