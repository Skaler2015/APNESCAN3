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
        var profilesPath = Path.Combine(Paths.AppData, "jit");
        Directory.CreateDirectory(profilesPath);
        ProfileOptimization.SetProfileRoot(profilesPath);
        ProfileOptimization.StartProfile("apnescan.jit");

        WinFormsEntryPoint.Run(args);
    }
}