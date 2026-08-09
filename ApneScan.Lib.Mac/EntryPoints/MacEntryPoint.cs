using ApneScan.EtoForms;
using ApneScan.EtoForms.Mac;
using ApneScan.Modules;

namespace ApneScan.EntryPoints;

/// <summary>
/// The entry point logic for the Mac ApneScan executable.
/// </summary>
public static class MacEntryPoint
{
    public static int Run(string[] args)
    {
        Runtime.MarshalManagedException += (_, eventArgs) =>
        {
            Log.ErrorException("Marshalling managed exception", eventArgs.Exception);
            eventArgs.ExceptionMode = MarshalManagedExceptionMode.ThrowObjectiveCException;
        };
        Runtime.MarshalObjectiveCException += (_, eventArgs) =>
        {
            Log.Error($"Marshalling ObjC exception: {eventArgs.Exception.Description}");
        };

        EtoPlatform.Current = new MacEtoPlatform();

        var subArgs = args.Skip(1).ToArray();
        return args switch
        {
            ["cli" or "console", ..] => ConsoleEntryPoint.Run(subArgs, new MacImagesModule(), new MacModule()),
            ["worker", ..] => MacWorkerEntryPoint.Run(subArgs),
            ["server", ..] => ServerEntryPoint.Run(subArgs, new MacImagesModule(), new MacModule()),
            _ => GuiEntryPoint.Run(args, new MacImagesModule(), new MacModule())
        };
    }
}