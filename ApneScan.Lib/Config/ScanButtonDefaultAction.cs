using ApneScan.Scan;

namespace ApneScan.Config;

public enum ScanButtonDefaultAction
{
    [LocalizedDescription(typeof(SettingsResources), "ScanButtonDefaultAction_ScanWithDefaultProfile")]
    ScanWithDefaultProfile,
    [LocalizedDescription(typeof(SettingsResources), "ScanButtonDefaultAction_AlwaysPrompt")]
    AlwaysPrompt
}