using ApneScan.Scan;

namespace ApneScan.Ocr;

public enum LocalizedOcrMode
{
    [LocalizedDescription(typeof(SettingsResources), "OcrMode_Fast")]
    Fast,
    [LocalizedDescription(typeof(SettingsResources), "OcrMode_Best")]
    Best,
    Legacy // Deprecated, not mapped to the Sdk OcrMode
}