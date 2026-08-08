using Eto.Drawing;

namespace ApneScan.ImportExport.Email;

public class EmailProviderWidget
{
    public required EmailProviderType ProviderType { get; init; }
    public Bitmap? ProviderIcon { get; init; }
    public required string ProviderIconName { get; init; }
    public required string ProviderName { get; init; }
    public required Func<bool> Choose { get; init; }
    public bool Enabled { get; set; } = true;
}