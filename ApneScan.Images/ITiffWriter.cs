using ApneScan.Util;

namespace ApneScan.Images;

public interface ITiffWriter
{
    bool SaveTiff(IList<IMemoryImage> images, string path,
        TiffCompressionType compression = TiffCompressionType.Auto, ProgressHandler progress = default);

    bool SaveTiff(IList<IMemoryImage> images, Stream stream,
        TiffCompressionType compression = TiffCompressionType.Auto, ProgressHandler progress = default);
}