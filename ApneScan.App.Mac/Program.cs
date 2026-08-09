using System.Runtime;
using ApneScan.EntryPoints;

namespace ApneScan;

static class Program
{
    /// <summary>
    /// The ApneScan.app main method.
    /// </summary>
    static void Main(string[] args)
    {
        var profilesPath = Path.Combine(Paths.AppData, "jit");
        Directory.CreateDirectory(profilesPath);
        ProfileOptimization.SetProfileRoot(profilesPath);
        ProfileOptimization.StartProfile("apnescan.jit");

        // Use reflection to avoid antivirus false positives (yes, really)
        typeof(MacEntryPoint).GetMethod("Run").Invoke(null, new object[] { args });
    }
}