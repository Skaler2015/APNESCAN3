using System.Threading;
using ApneScan.Scan;

namespace ApneScan.Ocr;

/// <summary>
/// Interface for OCR (optical character recognition). See TesseractOcrEngine.
/// </summary>
public interface IOcrEngine
{
    Task<OcrResult?> ProcessImage(ScanningContext scanningContext, string imagePath, OcrParams ocrParams,
        CancellationToken cancelToken);
}