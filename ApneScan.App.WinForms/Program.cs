using System.Runtime;
using ApneScan.EntryPoints;

namespace ApneScan;

static class Program
{
    /// <summary>
    /// The ApneScan.exe main method.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) => LogStartupError(e.ExceptionObject as Exception);
        try
        {
            var profilesPath = Path.Combine(Paths.AppData, "jit");
            Directory.CreateDirectory(profilesPath);
            ProfileOptimization.SetProfileRoot(profilesPath);
            ProfileOptimization.StartProfile("apnescan.jit");

            WinFormsEntryPoint.Run(args);
        }
        catch (Exception ex)
        {
            LogStartupError(ex);
            throw;
        }
    }

    // Writes any fatal startup error to %AppData%\ApneScan\startup-error.log so crashes that
    // close the window immediately can still be diagnosed.
    private static void LogStartupError(Exception? ex)
    {
        try
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ApneScan");
            Directory.CreateDirectory(dir);
            File.AppendAllText(Path.Combine(dir, "startup-error.log"),
                $"{DateTime.Now:u}{Environment.NewLine}{ex}{Environment.NewLine}{Environment.NewLine}");
        }
        catch
        {
            // Nothing more we can do if even logging fails.
        }
    }
}