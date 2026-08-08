using ApneScan.Scan;

namespace ApneScan.ImportExport;

/// <summary>
/// Additional parameters for importing files (e.g. PDF password, barcode detection, thumbnail rendering).
/// </summary>
public class ImportParams
{
    public ImportParams()
    {
        Slice = Slice.Default;
    }

    public string? Password { get; set; }

    public Slice Slice { get; set; }

    public BarcodeDetectionOptions BarcodeDetectionOptions { get; set; } = new BarcodeDetectionOptions();

    public int? ThumbnailSize { get; set; }
}