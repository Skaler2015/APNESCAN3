using ApneScan.Scan;

namespace ApneScan.Config;

public enum SaveButtonDefaultAction
{
    [LocalizedDescription(typeof(SettingsResources), "SaveButtonDefaultAction_SaveAll")]
    SaveAll,
    [LocalizedDescription(typeof(SettingsResources), "SaveButtonDefaultAction_SaveSelected")]
    SaveSelected,
    [LocalizedDescription(typeof(SettingsResources), "SaveButtonDefaultAction_AlwaysPrompt")]
    AlwaysPrompt,
    [LocalizedDescription(typeof(SettingsResources), "SaveButtonDefaultAction_PromptIfSelected")]
    PromptIfSelected
}