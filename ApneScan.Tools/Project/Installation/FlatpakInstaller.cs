using ApneScan.Tools.Project.Targets;

namespace ApneScan.Tools.Project.Installation;

public static class FlatpakInstaller
{
    public static void Install(Platform platform, string version, bool run)
    {
        var flatpakPath = ProjectHelper.GetPackagePath("flatpak", platform, version);
        Output.Info($"Starting flatpak install for: {flatpakPath}");

        try
        {
            Cli.Run("flatpak", $"uninstall --user --noninteractive com.apnescan.ApneScan");
        }
        catch (Exception)
        {
            // Ok if uninstall fails
        }

        Cli.Run("flatpak", $"install --user --noninteractive {flatpakPath}");
        if (run)
        {
            Cli.Run("flatpak", $"run com.apnescan.ApneScan");
        }

        Output.Info("Installed.");
    }
}