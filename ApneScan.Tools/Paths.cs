using System.Reflection;

namespace ApneScan.Tools;

internal static class Paths
{
    private static string? _root;

    public static string SolutionRoot
    {
        get
        {
            if (_root == null)
            {
                _root = Assembly.GetExecutingAssembly().Location;
                while (!File.Exists(Path.Combine(_root, "ApneScan.sln")))
                {
                    _root = Path.GetDirectoryName(_root);
                    if (_root == null)
                    {
                        throw new Exception("Couldn't find ApneScan folder");
                    }
                }
            }

            return _root;
        }
    }

    public static string Setup => Path.Combine(SolutionRoot, "ApneScan.Setup");

    public static string SetupWindows => Path.Combine(Setup, "config", "windows");

    public static string SetupLinux => Path.Combine(Setup, "config", "linux");
    
    public static string SetupObj => Path.Combine(Setup, "obj");
    
    public static string Publish => Path.Combine(Setup, "publish");

    public static string ApneScanUserFolder =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".apnescan");

    public static string ConfigFile => Path.Combine(SolutionRoot, "ApneScan.Tools", "n2-config.json");

    public static string PoFolder => Path.Combine(SolutionRoot, "ApneScan.Lib", "Lang", "po");

    public static string TemplatesFile => Path.Combine(PoFolder, "templates.pot");
}