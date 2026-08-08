using ApneScan.Scan;

namespace ApneScan.EtoForms.Desktop;

public interface IDesktopScanController
{
    Task ScanWithDevice(string deviceID);
    Task ScanDefault();
    Task ScanWithNewProfile();
    Task ScanWithProfile(ScanProfile profile);
}