using ApneScan.Tools.Project.Targets;

namespace ApneScan.Tools.Project.Packaging;

public static class RpmPackager
{
    public static void PackageRpm(PackageInfo pkgInfo, bool noSign)
    {
        var rpmPath = pkgInfo.GetPath("rpm");
        Output.Info($"Packaging rpm: {rpmPath}");

        Output.Verbose("Building binaries");
        var runtimeId = pkgInfo.Platform == Platform.LinuxArm ? "linux-arm64" : "linux-x64";
        Cli.Run("dotnet", $"clean ApneScan.App.Gtk -c Release -r {runtimeId}");
        Cli.Run("dotnet",
            $"publish ApneScan.App.Gtk -c Release -r {runtimeId} --self-contained /p:DebugType=None /p:DebugSymbols=false");

        Output.Verbose("Creating package");

        var workingDir = Path.Combine(Paths.SetupObj, "rpm");
        if (Directory.Exists(workingDir))
        {
            Directory.Delete(workingDir, true);
        }

        Directory.CreateDirectory(workingDir);

        foreach (var subdir in new[] { "RPMS", "SRPMS", "BUILD", "SOURCES", "SPECS", "tmp" })
        {
            Directory.CreateDirectory(Path.Combine(workingDir, subdir));
        }

        var dirArg = $"-D \"_topdir {workingDir}\" -D \"_tmppath {workingDir}/tmp\"";

        // Create spec file
        var template = File.ReadAllText(Path.Combine(Paths.SetupLinux, "rpm-spec"));
        template = template.Replace("{!version}", pkgInfo.VersionNumber);
        File.WriteAllText(Path.Combine(workingDir, "SPECS/apnescan.spec"), template);

        // Copy binary files
        var publishDir = Path.Combine(Paths.SolutionRoot, "ApneScan.App.Gtk", "bin", "Release", "net10.0", runtimeId,
            "publish");
        var filesDir = Path.Combine(workingDir, $"apnescan-{pkgInfo.VersionNumber}");
        var targetDir = Path.Combine(filesDir, "usr/lib/apnescan");
        ProjectHelper.CopyDirectory(publishDir, targetDir);

        // Copy metadata files
        var iconDir = Path.Combine(filesDir, "usr/share/icons/hicolor/128x128/apps");
        Directory.CreateDirectory(iconDir);
        var appsDir = Path.Combine(filesDir, "usr/share/applications");
        Directory.CreateDirectory(appsDir);
        var metainfoDir = Path.Combine(filesDir, "usr/share/metainfo");
        Directory.CreateDirectory(metainfoDir);
        File.Copy(
            Path.Combine(Paths.SolutionRoot, "ApneScan.Lib", "Icons", "scanner-128.png"),
            Path.Combine(iconDir, "com.apnescan.ApneScan.png"));
        File.Copy(
            Path.Combine(Paths.SetupLinux, "com.apnescan.ApneScan.desktop"),
            Path.Combine(appsDir, "apnescan.desktop"));
        File.WriteAllText(Path.Combine(metainfoDir, "com.apnescan.ApneScan.metainfo.xml"),
            ProjectHelper.GetLinuxMetaInfo(pkgInfo));
        File.Copy(
            Path.Combine(Paths.SolutionRoot, "LICENSE"),
            Path.Combine(targetDir, "LICENSE.txt"));

        // Create symlinks
        var binDir = Path.Combine(filesDir, "usr/bin");
        Directory.CreateDirectory(binDir);
        Cli.Run("ln", $"-s /usr/lib/apnescan/apnescan {Path.Combine(binDir, "apnescan")}");

        // Compress files
        Cli.Run("tar", $"-zcvf {workingDir}/SOURCES/apnescan-{pkgInfo.VersionNumber}.tar.gz {Path.GetFileName(filesDir)}",
            workingDir: workingDir);

        // Build RPM
        var arch = pkgInfo.Platform == Platform.LinuxArm ? "aarch64" : "x86_64";
        Cli.Run("rpmbuild", $"{dirArg} -ba --target {arch} {workingDir}/SPECS/apnescan.spec");
        var sourceRpmPath = Path.Combine(workingDir, $"RPMS/{arch}/apnescan-{pkgInfo.VersionNumber}-1.{arch}.rpm");

        // Sign
        if (!noSign)
        {
            Cli.Run("rpmsign", $"--addsign {sourceRpmPath}");
        }

        // Copy to output
        File.Copy(sourceRpmPath, rpmPath, true);

        Output.OperationEnd($"Packaged rpm: {rpmPath}");
    }
}