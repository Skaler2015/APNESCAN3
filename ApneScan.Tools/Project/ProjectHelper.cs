using System.Text.RegularExpressions;
using System.Threading;
using ApneScan.Tools.Project.Packaging;
using ApneScan.Tools.Project.Targets;

namespace ApneScan.Tools.Project;

public static class ProjectHelper
{
    public static string GetCurrentVersion()
    {
        var versionTargetsPath = Path.Combine(Paths.Setup, "targets", "VersionTargets.targets");
        var versionTargetsFile = XDocument.Load(versionTargetsPath);
        var version = versionTargetsFile.Descendants().SingleOrDefault(x => x.Name.LocalName == "Version")?.Value;
        if (version == null)
        {
            throw new Exception($"Could not read version from project: {versionTargetsPath}");
        }
        return version;
    }

    public static string GetCurrentVersionName()
    {
        var versionTargetsPath = Path.Combine(Paths.Setup, "targets", "VersionTargets.targets");
        var versionTargetsFile = XDocument.Load(versionTargetsPath);
        var version = versionTargetsFile.Descendants().SingleOrDefault(x => x.Name.LocalName == "VersionName")?.Value;
        if (version == null)
        {
            throw new Exception($"Could not read version from project: {versionTargetsPath}");
        }
        return version;
    }

    public static string GetSdkVersion()
    {
        var sdkTargetsPath = Path.Combine(Paths.Setup, "targets", "SdkPackageTargets.targets");
        var sdkTargetsFile = XDocument.Load(sdkTargetsPath);
        var version = sdkTargetsFile.Descendants().SingleOrDefault(x => x.Name.LocalName == "PackageVersion")?.Value;
        if (version == null)
        {
            throw new Exception($"Could not read sdk version: {sdkTargetsPath}");
        }
        return version;
    }

    public static string[] GetSdkProjects() => new[]
    {
        "ApneScan.Sdk",
        "ApneScan.Sdk.Worker.Win32",
        "ApneScan.Escl",
        "ApneScan.Escl.Server",
        "ApneScan.Internals",
        "ApneScan.Images",
        "ApneScan.Images.Gdi",
        "ApneScan.Images.Gtk",
        "ApneScan.Images.Mac",
        "ApneScan.Images.ImageSharp",
        "ApneScan.Images.Wpf",
    };

    public static string GetPackagePath(string ext, Platform platform, string? version = null,
        string? packageName = null)
    {
        version ??= GetCurrentVersionName();
        packageName ??= platform.PackageName();
        var path = Path.Combine(Paths.Publish, version, $"apnescan-{version}-{packageName}.{ext}");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        return path;
    }

    public static string GetInstallationFolder(Platform platform)
    {
        var pfVar = "%PROGRAMFILES%";
        var pfPath = Environment.ExpandEnvironmentVariables(pfVar);
        return Path.Combine(pfPath, "ApneScan");
    }

    public static void DeleteInstallationFolder(Platform platform)
    {
        var folder = GetInstallationFolder(platform);
        if (Directory.Exists(folder))
        {
            Output.Info($"Deleting old installation: {folder}");
            try
            {
                Directory.Delete(folder, true);
            }
            catch (UnauthorizedAccessException e)
            {
                // TODO: Once we add more complex workflows, we should verify elevation before running the workflow
                throw new Exception("This command requires administrator permissions to run.", e);
            }
        }
    }

    public static void CloseMostRecentApneScan()
    {
        for (int i = 0; i < 20; i++)
        {
            var proc = Process.GetProcessesByName("ApneScan")
                .Where(x => x.StartTime > DateTime.Now - TimeSpan.FromSeconds(2))
                .OrderBy(x => x.StartTime)
                .FirstOrDefault();
            if (proc != null)
            {
                proc.Kill();
                break;
            }
            Thread.Sleep(100);
        }
    }

    public static void CopyDirectory(string sourceDir, string destinationDir)
    {
        
        var dir = new DirectoryInfo(sourceDir);
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
        DirectoryInfo[] dirs = dir.GetDirectories();
        Directory.CreateDirectory(destinationDir);
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }
        foreach (DirectoryInfo subDir in dirs)
        {
            string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir);
        }
    }

    public static string GetLinuxMetaInfo(PackageInfo packageInfo)
    {
        // Update metainfo file with the current version/date
        var metaInfo = File.ReadAllText(Path.Combine(Paths.SetupLinux, "com.apnescan.ApneScan.metainfo.xml"));
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        metaInfo = Regex.Replace(metaInfo,
            @"<release [^>]+/>",
            $"<release version=\"{packageInfo.VersionName}\" date=\"{date}\" />");
        return metaInfo;
    }
}