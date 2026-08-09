namespace ApneScan.Sdk.Tests;

public static class TestImageContextFactory
{
    public static ImageContext Get()
    {
        return Environment.GetEnvironmentVariable("ApneScan_TEST_IMAGES") switch
        {
#if WINDOWS
            "gdi" => new ApneScan.Images.Gdi.GdiImageContext(),
            "wpf" => new ApneScan.Images.Wpf.WpfImageContext(),
#endif
            "is" or "imagesharp" => new ApneScan.Images.ImageSharp.ImageSharpImageContext(),
#if MAC
            "mac" => new ApneScan.Images.Mac.MacImageContext(),
#endif
#if LINUX
            "gtk" or "gdk" or "linux" => new ApneScan.Images.Gtk.GtkImageContext(),
#endif
            _ =>
#if MAC
                new ApneScan.Images.Mac.MacImageContext()
#elif LINUX
                new ApneScan.Images.Gtk.GtkImageContext()
#else
                new ApneScan.Images.Gdi.GdiImageContext()
#endif
        };
    }
}