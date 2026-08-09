using System.Threading;

namespace ApneScan.Images;

/// <summary>
/// Represents information about an image obtained during post-processing (e.g. thumbnail image, barcode).
/// </summary>
public record PostProcessingData(
    IMemoryImage? Thumbnail,
    TransformState? ThumbnailTransformState,
    int PageNumber,
    PageSide PageSide,
    Barcode Barcode,
    CancellationTokenSource? OcrCts,
    string? OriginalFilePath)
{
    public PostProcessingData() : this(null, null, 0, PageSide.Unknown, Barcode.NoDetection, null, null)
    {
    }
}