namespace ApneScan.Scan.Internal;

internal interface IScanBridgeFactory
{
    IScanBridge Create(ScanOptions options);
}