namespace ApneScan.Scan.Internal;

internal interface IScanDriverFactory
{
    IScanDriver Create(ScanOptions options);
}