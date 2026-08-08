using ApneScan.Scan;
using ApneScan.Scan.Internal;

namespace ApneScan.Sdk.Tests.Mocks;

internal class InProcScanBridgeFactory : IScanBridgeFactory
{
    private readonly InProcScanBridge _inProcScanBridge;

    public InProcScanBridgeFactory(InProcScanBridge inProcScanBridge)
    {
        _inProcScanBridge = inProcScanBridge;
    }
        
    public IScanBridge Create(ScanOptions options)
    {
        return _inProcScanBridge;
    }
}