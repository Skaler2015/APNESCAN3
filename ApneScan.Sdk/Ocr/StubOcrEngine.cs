using System.Threading;
using ApneScan.Scan;

namespace ApneScan.Ocr;

internal class StubOcrEngine : IOcrEngine
{
    public Task<OcrResult?> ProcessImage(ScanningContext scanningContext, string imagePath, OcrParams ocrParams,
        CancellationToken cancelToken)
    {
        return Task.FromResult<OcrResult?>(null);
    }
}