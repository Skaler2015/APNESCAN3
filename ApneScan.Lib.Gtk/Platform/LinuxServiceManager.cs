namespace ApneScan.Platform;

/// <summary>
/// Manages a user-level systemd service on Linux.
/// </summary>
public class LinuxServiceManager : IOsServiceManager
{
    // At the moment we only support systemd on Linux and without Flatpak
    public bool CanRegister => Directory.Exists("/run/systemd/system/") && !File.Exists("/.flatpak-info");

    private static string UnitPath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config/systemd/user/apnescan-sharing-server.service");
    
    public bool IsRegistered => File.Exists(UnitPath);

    public bool Register()
    {
        var unitDef = $"""
                          [Unit]
                          Description=ApneScan Scanner Sharing Server
                          
                          [Service]
                          Type=simple
                          Restart=always
                          RestartSec=1
                          ExecStart={Environment.ProcessPath} server
                          KillMode=process
                          
                          [Install]
                          WantedBy=default.target
                          """;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(UnitPath)!);
            File.WriteAllText(UnitPath, unitDef);
        }
        catch (Exception ex)
        {
            Log.ErrorException($"Error creating systemd unit: {UnitPath}", ex);
        }
        if (!ProcessHelper.TryRun("systemctl", "--user daemon-reload", 1000))
        {
            Log.Error("Could not run systemctl daemon-reload");
        }
        if (!ProcessHelper.TryRun("systemctl", "--user enable apnescan-sharing-server", 1000))
        {
            Log.Error("Could not enable service apnescan-sharing-server");
        }
        if (!ProcessHelper.TryRun("systemctl", "--user start apnescan-sharing-server", 1000))
        {
            Log.Error("Could not start service apnescan-sharing-server");
            return false;
        }
        return true;
    }

    public void Unregister()
    {
        if (!ProcessHelper.TryRun("systemctl", "--user stop apnescan-sharing-server", 1000))
        {
            Log.Error("Could not stop service apnescan-sharing-server");
        }
        if (!ProcessHelper.TryRun("systemctl", "--user disable apnescan-sharing-server", 1000))
        {
            Log.Error("Could not disable service apnescan-sharing-server");
        }
        try
        {
            File.Delete(UnitPath);
        }
        catch (Exception ex)
        {
            Log.ErrorException($"Error deleting systemd unit: {UnitPath}", ex);
        }
        if (!ProcessHelper.TryRun("systemctl", "--user daemon-reload", 1000))
        {
            Log.Error("Could not run systemctl daemon-reload");
        }
    }
}