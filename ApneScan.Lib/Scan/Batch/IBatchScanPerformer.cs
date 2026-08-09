using System.Threading;
using ApneScan.EtoForms;

namespace ApneScan.Scan.Batch;

public interface IBatchScanPerformer
{
    Task PerformBatchScan(BatchSettings settings, IFormBase batchForm, Action<ProcessedImage> imageCallback, Action<string> progressCallback, CancellationToken cancelToken);
}