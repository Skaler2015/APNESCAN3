using ApneScan.EtoForms;
using ApneScan.EtoForms.WinForms;
using ApneScan.Modules;

namespace ApneScan.EntryPoints;

/// <summary>
/// The entry point logic for ApneScan.exe, the ApneScan GUI.
/// </summary>
public static class WinFormsEntryPoint
{
    public static int Run(string[] args)
    {
        EtoPlatform.Current = new WinFormsEtoPlatform();

        var subArgs = args.Skip(1).ToArray();
        return args switch
        {
            ["worker", ..] => WindowsNativeWorkerEntryPoint.Run(subArgs),
            ["server", ..] => ServerEntryPoint.Run(subArgs, new GdiModule(), new WinFormsModule()),
            _ => GuiEntryPoint.Run(args, new GdiModule(), new WinFormsModule())
        };
    }
}