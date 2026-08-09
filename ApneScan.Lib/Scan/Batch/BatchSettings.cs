using ApneScan.ImportExport;

namespace ApneScan.Scan.Batch;

public class BatchSettings
{
    public string? ProfileDisplayName { get; set; }

    public BatchScanType ScanType { get; set; }

    public int ScanCount { get; set; }

    public double ScanIntervalSeconds { get; set; }

    public BatchOutputType OutputType { get; set;  }

    public SaveSeparator SaveSeparator { get; set; }

    public string? SavePath { get; set; }
}