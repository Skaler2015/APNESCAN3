using ApneScan.Scan;

namespace ApneScan.EtoForms.Widgets;

public class DeviceChangedEventArgs : EventArgs
{
    public required DeviceChoice PreviousChoice { get; init; }

    public required DeviceChoice NewChoice { get; init; }
}