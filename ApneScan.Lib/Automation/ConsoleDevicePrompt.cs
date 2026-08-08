using ApneScan.Scan;

namespace ApneScan.Automation;

public class ConsoleDevicePrompt : IDevicePrompt
{
    public Task<DeviceChoice> PromptForDevice(ScanOptions options, bool allowAlwaysAsk)
    {
        // TODO: What's best to do here? Use a GUI prompt? A console prompt? Or just do nothing like this?
        return Task.FromResult(DeviceChoice.None);
    }
}