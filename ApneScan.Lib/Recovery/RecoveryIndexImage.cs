using ApneScan.Scan;

namespace ApneScan.Recovery;

public class RecoveryIndexImage
{
    public string? FileName { get; set; }

    public List<Transform>? TransformList { get; set; }

    public bool HighQuality { get; set; }

    public string? PageSize { get; set; }
}