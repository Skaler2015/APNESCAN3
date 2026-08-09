using ApneScan.Images.Gdi;
using ApneScan.Remoting.Worker;
using ApneScan.Scan;

namespace ApneScan.Sdk.Worker;

[System.Runtime.Versioning.SupportedOSPlatform("windows7.0")]
public class Program
{
    public static async Task Main()
    {
        var scanningContext = new ScanningContext(new GdiImageContext());
        await WorkerServer.Run(scanningContext);
    }
}