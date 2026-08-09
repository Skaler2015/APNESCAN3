using ApneScan.EtoForms;
using ApneScan.EtoForms.WinForms;
using ApneScan.Modules;

namespace ApneScan.EntryPoints;

/// <summary>
/// The entry point for ApneScan.Console.exe, the ApneScan CLI.
/// </summary>
public static class WindowsConsoleEntryPoint
{
    public static int Run(string[] args)
    {
        EtoPlatform.Current = new WinFormsEtoPlatform();
        return ConsoleEntryPoint.Run(args, new GdiModule(), new WinFormsModule());
    }
}