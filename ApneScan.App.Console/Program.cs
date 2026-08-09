using System.Runtime;
using ApneScan.EntryPoints;

namespace ApneScan.Console;

static class Program
{
    /// <summary>
    /// The ApneScan.Console.exe main method.
    /// </summary>
    [STAThread]
    static int Main(string[] args)
    {
        var profilesPath = Path.Combine(Paths.AppData, "jit");
        Directory.CreateDirectory(profilesPath);
        ProfileOptimization.SetProfileRoot(profilesPath);
        ProfileOptimization.StartProfile("apnescan.console.jit");

        return WindowsConsoleEntryPoint.Run(args);
    }
}