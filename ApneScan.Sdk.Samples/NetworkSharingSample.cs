using ApneScan.Escl.Server;
using ApneScan.Images.Gdi;
using ApneScan.Remoting.Server;
using ApneScan.Scan;

namespace ApneScan.Sdk.Samples;

public class NetworkSharingSample
{
    public static async Task Server()
    {
        // ApneScan can share scanners across the local network using the ESCL protocol with the ApneScan.Escl.Server package.
        // On the server, you need to set up ScanServer with the device(s) to share.
        // On the client, you just scan as usual using Driver.Escl.

        using var scanningContext = new ScanningContext(new GdiImageContext());

        // Get the device to share
        var controller = new ScanController(scanningContext);
        ScanDevice device = (await controller.GetDeviceList()).First();

        // Set up the server (you'll need to reference ApneScan.Escl.Server to be able to create an EsclServer object).
        using var scanServer = new ScanServer(scanningContext, new EsclServer());

        // Register a device to be shared
        scanServer.RegisterDevice(device);

        // Run the server until the user presses Enter
        await scanServer.Start();
        Console.ReadLine();
        await scanServer.Stop();
    }

    public static async Task Client()
    {
        using var scanningContext = new ScanningContext(new GdiImageContext());
        var controller = new ScanController(scanningContext);

        // Find the shared device using Driver.Escl
        ScanDevice device = (await controller.GetDeviceList(Driver.Escl)).First();

        // Set up options
        var options = new ScanOptions { Device = device };

        // Do the scan
        await foreach (var image in controller.Scan(options))
        {
            Console.WriteLine("Scanned a page!");
        }
    }
}