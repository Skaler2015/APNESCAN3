using ApneScan.Scan;

namespace ApneScan.ImportExport.Images;

public enum TiffCompression
{
    [LocalizedDescription(typeof(SettingsResources), "TiffComp_Auto")]
    Auto,
    [LocalizedDescription(typeof(SettingsResources), "TiffComp_Lzw")]
    Lzw,
    [LocalizedDescription(typeof(SettingsResources), "TiffComp_Ccitt4")]
    Ccitt4,
    [LocalizedDescription(typeof(SettingsResources), "TiffComp_None")]
    None
}