using System.Threading;

namespace ApneScan.Scan.Internal.Twain;

/// <summary>
/// Stub implementation of ITwainController for unsupported platforms.
/// </summary>
internal class StubTwainController : ITwainController
{
    public Task<List<ScanDevice>> GetDeviceList(ScanOptions options)
    {
        throw new NotSupportedException();
    }

    public Task<ScanCaps> GetCaps(ScanOptions options)
    {
        throw new NotSupportedException();
    }

    public Task StartScan(ScanOptions options, ITwainEvents twainEvents, CancellationToken cancelToken)
    {
        throw new NotSupportedException();
    }
}