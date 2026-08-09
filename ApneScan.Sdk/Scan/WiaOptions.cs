namespace ApneScan.Scan;

/// <summary>
/// Scanning options specific to the WIA driver.
/// </summary>
public class WiaOptions
{
    public WiaApiVersion WiaApiVersion { get; set; }

    public bool OffsetWidth { get; set; }
}