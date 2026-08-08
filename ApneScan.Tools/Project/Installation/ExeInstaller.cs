using ApneScan.Tools.Project.Targets;

namespace ApneScan.Tools.Project.Installation;

public static class ExeInstaller
{
    public static void Install(Platform platform, string version, bool run)
    {
        ProjectHelper.DeleteInstallationFolder(Platform.Win64);

        var exePath = ProjectHelper.GetPackagePath("exe", platform, version);
        Output.Info($"Starting exe installer: {exePath}");

        var process = Process.Start(new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = "/SILENT /CLOSEAPPLICATIONS"
        });
        if (process == null)
        {
            throw new Exception($"Could not start installer: {exePath}");
        }
        process.WaitForExit();

        if (!run)
        {
            ProjectHelper.CloseMostRecentApneScan();
        }

        Output.Info("Installed.");
    }
}